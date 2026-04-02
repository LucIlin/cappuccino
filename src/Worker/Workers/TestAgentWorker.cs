using System.ClientModel;

using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using Worker.Settings;

namespace Worker.Workers;

public class TestAgentWorker : BackgroundService
{
    private readonly LLMProfile _llmProfile;
    private static readonly string _profile = "TestProfile";
    
    public TestAgentWorker(IOptions<LLMConfiguration> llmConfiguration)
    {
        _llmProfile = llmConfiguration.Value.GetProfile(_profile);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ChatClientAgent agent = new OpenAIClient(new ApiKeyCredential(_llmProfile.ApiKey))
            .GetChatClient(_llmProfile.Model)
            .AsAIAgent(
                instructions: "You are a helpful assistant that always ends his messages with: \"Amazing question, Lucas!\"",
                name: "Agent Asskisser");

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Ask your helpful assistant a question.");
            Console.WriteLine("Question: ");
            
            string question = Console.ReadLine() ?? "";
            
            AgentResponse response = await agent.RunAsync(question);
            
            Console.WriteLine(response);
        }
        
    }
}