using System.ClientModel;
using Agents.LLM;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OpenAI;
using Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

var llmConfiguration = builder.Configuration
    .GetSection("LLMConfiguration")
    .Get<LLMConfiguration>()
    ?? throw new ArgumentNullException();

var chatClientFactory = ChatClientFactory.BuildFactory(factory =>
{
    factory.Register(
        "OllamaLocalTest",
        profile => new OllamaApiClient(new Uri(profile.BaseUrl), profile.Model));
    
    factory.Register(
        "OpenAITest",
        profile => new OpenAIClient(new ApiKeyCredential(profile.ApiKey))
            .GetChatClient(profile.Model)
            .AsIChatClient());
});

builder.Services.AddSingleton(llmConfiguration);
builder.Services.AddSingleton(chatClientFactory);
builder.Services.AddHostedService<TestWorker>();

var host = builder.Build();

host.Run();