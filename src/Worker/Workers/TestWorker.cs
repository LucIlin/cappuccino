
using Agents;
using Agents.Communicator;
using Agents.LLM;
using Microsoft.Agents.AI;

namespace Worker.Workers;

public class TestWorker : BackgroundService
{
    private readonly LLMConfiguration _llmConfigs;
    private readonly ChatClientFactory _chatClientFactory;
    private static readonly string _profile = "openai_api_gpt-5-nano";

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
        var agentFactory = new ChatClientPromptAgentFactory(chatClient);
        var agent = await agentFactory.CreateFromYamlAsync(AgentDefinitionLoader.Load("Communicator"));
        
        while(!stoppingToken.IsCancellationRequested)
        {
            Console.Write("User prompt: ");
            var response = await agent.RunAsync<CommunicatorResponse>(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("Status: " + response.Result.Status);
            Console.WriteLine("Response: " + response);
        }
    }
}