using Microsoft.Agents.AI.Workflows;
using Workflows.Executors;

namespace Workflows;

public static class CommunicationWorkflow
{
    public static Workflow Build(CommunicatorExecutor communicator)
    {
        return new WorkflowBuilder(communicator)
            .WithOutputFrom(communicator)
            .Build();
    }
}
       