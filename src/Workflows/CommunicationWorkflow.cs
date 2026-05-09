using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Workflows.Executors;

namespace Workflows;

public static class CommunicationWorkflow
{
    public static Workflow Build(AIHostAgent communicatorAgent)
    {
        var communicator = new CommunicatorExecutor(communicatorAgent);
        return new WorkflowBuilder(communicator)
            .WithOutputFrom(communicator)
            .Build();
    }
}
       