using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Agents.Communicator;
using Microsoft.Extensions.AI;
using Workflows.Messages;

namespace Workflows.Executors;

[YieldsOutput(typeof(UserFacingMessage))]
[YieldsOutput(typeof(MonitoringRequest))]
public sealed partial class CommunicatorExecutor([FromKeyedServices("Communicator")]AIHostAgent agent) : Executor("Communicator")
{
    [MessageHandler]
    private async ValueTask HandleAsync(UserInput input, IWorkflowContext context)
    {
        AgentSession session = await agent.GetOrCreateSessionAsync(input.ConversationId);
        
        AgentResponse<CommunicatorResponse> response = await agent.RunAsync<CommunicatorResponse>(
            new ChatMessage(ChatRole.User, input.Text),
            session);
        
        await agent.SaveSessionAsync(input.ConversationId, session);

        if (response.Result.Status == CommunicatorStatus.None)
            throw new InvalidOperationException(
                "Communicator returned a response with no status set");
        await context.YieldOutputAsync(new UserFacingMessage(response.Result.Message));
        
        if (response.Result.Status == CommunicatorStatus.Confirmed)
            await context.YieldOutputAsync(
                new MonitoringRequest(
                    response.Result.Summary
                    ?? throw new InvalidOperationException(
                        "Communicator returned a confirmed response with null Summary")));
    }
}