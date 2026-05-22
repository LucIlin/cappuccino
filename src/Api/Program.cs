using System.ClientModel;
using Agents;
using Agents.LLM;
using Api;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;
using Shared.Grpc;

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

var communicator = await agentBuilder.BuildAsync(
    "Communicator",
    llmConfiguration.GetProfile("openai_api_gpt-5.4"));

builder.Services.AddGrpcClient<FulfillmentService.FulfillmentServiceClient>(o =>
{
    o.Address = new Uri(
        builder.Configuration["FulfillmentGrpc:Address"]
        ?? "http://localhost:5014");
});

builder.Services.AddKeyedSingleton("Communicator", communicator);
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