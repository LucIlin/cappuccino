using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Workflows.Executors;

namespace Workflows;

public static class CommunicationWorkflow
{
    public static Workflow Build(AIAgent communicatorAgent)
    {
        var communicator = new CommunicatorExecutor(communicatorAgent);
        return new WorkflowBuilder(communicator)
            .WithOutputFrom(communicator)
            .Build();
    }
}
       