using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.ConfigStore;
using Workflows.Executors;
using Workflows.Paths;

namespace Workflows;

public static class MonitoringWorkflow
{
    public static Workflow Build(
        AIAgent analystAgent,
        AIAgent apiProvisionerAgent,
        IMonitoringConfigStore configStore,
        ILoggerFactory loggerFactory)
    {
        var analyst = new AnalystExecutor(analystAgent);
        var apiProvisioner = new ApiProvisionerExecutor(
            apiProvisionerAgent,
            configStore,
            loggerFactory.CreateLogger<ApiProvisionerExecutor>());

        return new WorkflowBuilder(analyst)
            .AddEdge<ApiMonitoringPath>(analyst, apiProvisioner, null)
            .WithOutputFrom(analyst)
            .WithOutputFrom(apiProvisioner)
            .Build();
    }
}