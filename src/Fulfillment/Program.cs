using System.ClientModel;
using System.Threading.Channels;
using Agents.LLM;
using Agents;
using Fulfillment.Services;
using Fulfillment.Tasks;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

var llmConfiguration = builder.Configuration
                           .GetSection("LLMConfiguration")
                           .Get<LLMConfiguration>()
                       ?? throw new ArgumentNullException();

var chatClientFactory = ChatClientFactory.BuildFactory(factory =>
{
    factory.Register(
        "ollama_local_dev",
        profile => new OllamaApiClient(new Uri(profile.BaseUrl), profile.Model));
    
    factory.Register(
        "openai_api_dev",
        profile => new OpenAIClient(new ApiKeyCredential(profile.ApiKey))
            .GetChatClient(profile.Model)
            .AsIChatClient());
});

var agentBuilder = new AgentBuilder(chatClientFactory);

var analyst = agentBuilder.BuildAsync(
    "Analyst",
    llmConfiguration.GetProfile("openai_api_gpt-5-nano"));
var apiProvisioner = agentBuilder.BuildAsync(
    "ApiProvisioner",
    llmConfiguration.GetProfile("openai_api_gpt-5-nano"));

builder.Services.AddSingleton(Channel.CreateUnbounded<MonitoringTask>(
    new UnboundedChannelOptions { SingleReader = true }));

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddKeyedSingleton("Analyst", analyst);
builder.Services.AddKeyedSingleton("ApiProvisioner", apiProvisioner);

var app = builder.Build();

app.MapGrpcService<FulfillmentGrpcService>();

app.Run();