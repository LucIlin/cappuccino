using Agents.LLM;
using Microsoft.Agents.AI;

namespace Agents;

public sealed class AgentBuilder
{
    private readonly ChatClientFactory _chatClientFactory;

    public AgentBuilder(ChatClientFactory chatClientFactory)
    {
        _chatClientFactory = chatClientFactory;
    }

    public async Task<AIAgent> BuildAsync(string agentName, LLMProfile profile)
    {
        var chatClient = _chatClientFactory.Create(profile);
        var agentFactory = new ChatClientPromptAgentFactory(chatClient);
        return await agentFactory.CreateFromYamlAsync(AgentDefinitionLoader.Load(agentName));
    }
}