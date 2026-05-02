using System.ClientModel;
using Agents.Agents;
using Agents.LLM;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
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
builder.Services.AddSingleton(llmConfiguration);
builder.Services.AddSingleton(chatClientFactory);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();