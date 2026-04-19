using Agents.LLM;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace Worker.Workers;

public class LocalOllamaTestWorker : BackgroundService
{
    private readonly LLMProfile _llmProfile;
    private static readonly string _profile = "LocalOllamaTest";

    public LocalOllamaTestWorker(IOptions<LLMConfiguration> llmConfiguration)
    {
        _llmProfile = llmConfiguration.Value.GetProfile(_profile);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var apiClient = new OllamaApiClient(new Uri($"{_llmProfile.BaseUrl}"), $"{_llmProfile.Model}");
        
        var chat = new Chat(apiClient);
        
        while(!stoppingToken.IsCancellationRequested)
        {
            Console.Write("You: ");
            var message = Console.ReadLine();

            Console.Write("Agent: ");
            await foreach(var responseToken in chat.SendAsync(message))
                Console.Write(responseToken);
        }
    }
}