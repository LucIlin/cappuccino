using System.Threading.Channels;
using Fulfillment.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.ConfigStore;
using Workflows;
using Workflows.Messages;
using Workflows.Paths;

namespace Fulfillment.Workers;

public class MonitoringWorker(
    Channel<MonitoringTask> Channel,
    ILogger<MonitoringWorker> Logger,
    ILoggerFactory LoggerFactory,
    [FromKeyedServices("Analyst")] AIAgent Analyst,
    [FromKeyedServices("ApiProvisioner")] AIAgent ApiProvisioner,
    IMonitoringConfigStore ConfigStore)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in Channel.Reader.ReadAllAsync(stoppingToken))
            _ = ProcessTaskAsync(task, stoppingToken);
    }

    private async Task ProcessTaskAsync(MonitoringTask task, CancellationToken ct)
    {
        Logger.LogInformation(
            "Processing monitoring task for conversation {Id}: {Description}",
            task.ConversationId, task.Description);

        var workflow = MonitoringWorkflow.Build(
            Analyst, ApiProvisioner, ConfigStore, LoggerFactory);

        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow,
            new MonitoringRequest(task.Description),
            cancellationToken: ct);

        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

        await foreach (WorkflowEvent evt in run.WatchStreamAsync().WithCancellation(ct))
        {
            switch (evt)
            {
                case WorkflowOutputEvent { Data: ProvisioningComplete complete }:
                    Logger.LogInformation(
                        "Monitoring provisioned for conversation {Id}: config {ConfigId}",
                        task.ConversationId, complete.ConfigId);
                    break;

                case WorkflowOutputEvent { Data: UnresolvableMonitoringPath unresolvable }:
                    Logger.LogWarning(
                        "Monitoring request for conversation {Id} could not be resolved: {Reason}",
                        task.ConversationId, unresolvable.Reason);
                    break;

                case WorkflowOutputEvent { Data: BrowserMonitoringPath }:
                case WorkflowOutputEvent { Data: ThirdPartyMonitoringPath }:
                    Logger.LogWarning(
                        "Path type not yet implemented for conversation {Id}",
                        task.ConversationId);
                    break;
            }
        }
    }
}