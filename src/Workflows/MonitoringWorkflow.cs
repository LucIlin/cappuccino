using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Workflows.Executors;

namespace Workflows;

public static class MonitoringWorkflow
{
    public static Workflow Build(AIAgent analystAgent)
    {
        var analyst = new AnalystExecutor(analystAgent);
        return new WorkflowBuilder(analyst)
            .WithOutputFrom(analyst)
            .Build();
    }
}