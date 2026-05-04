using Microsoft.Agents.AI.Workflows;
using Workflows.Executors;

namespace Workflows;

public static class NotificationWorkflow
{
    public static Workflow Build(CommunicatorExecutor communicator)
    {
        return new WorkflowBuilder(communicator)
            .WithOutputFrom(communicator)
            .Build();
    }
}
       