using Agents.LLM;
using Microsoft.Agents.AI;

namespace Worker.Workers;

public class TestWorker : BackgroundService
{
    private readonly LLMConfiguration _llmConfigs;
    private readonly ChatClientFactory _chatClientFactory;
    private static readonly string _profile = "ollama_local_llama3.1-8b";

    public TestWorker(
        LLMConfiguration llmConfigs,
        ChatClientFactory chatClientFactory)
    {
        _llmConfigs = llmConfigs;
        _chatClientFactory = chatClientFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var chatClient = _chatClientFactory.Create(_llmConfigs.GetProfile(_profile));
        
        while(!stoppingToken.IsCancellationRequested)
        {

        }
    }
}