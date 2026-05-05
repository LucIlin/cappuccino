using System.ClientModel;
using System.Threading.Channels;
using Agents.Agents;
using Agents.LLM;
using Api;
using Api.Tasks;
using Workflows;
using Workflows.Executors;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
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

var chatClient = chatClientFactory.Create(llmConfiguration.GetProfile("openai_api_gpt-5-nano"));
var agentFactory = new ChatClientPromptAgentFactory(chatClient);

var communicatorAgent = await agentFactory.CreateFromYamlAsync(AgentDefinitionLoader.Load("Communicator"));
var communicatorHost = new AIHostAgent(communicatorAgent, new InMemoryAgentSessionStore());

builder.Services.AddKeyedSingleton("Communicator",  communicatorHost);
builder.Services.AddSingleton<CommunicatorExecutor>();
builder.Services.AddSingleton<Workflow>(sp =>
    CommunicationWorkflow.Build( 
        sp.GetRequiredService<CommunicatorExecutor>()));
builder.Services.AddSingleton(llmConfiguration);
builder.Services.AddSingleton(chatClientFactory);
builder.Services.AddSingleton(Channel.CreateUnbounded<ErrandFulfillmentTask>(
    new UnboundedChannelOptions { SingleReader = true }));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapChatEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();