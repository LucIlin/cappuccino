using System.Threading.Channels;
using Fulfillment.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Workflows;
using Workflows.Messages;

namespace Fulfillment.Workers;

public class MonitoringWorker(
    Channel<MonitoringTask> Channel,
    ILogger<MonitoringWorker> Logger,
    [FromKeyedServices("Analyst")] AIAgent Analyst)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in Channel.Reader.ReadAllAsync(stoppingToken))
            _ = ProcessTaskAsync(task, stoppingToken);
    }

    private async Task ProcessTaskAsync(MonitoringTask task, CancellationToken stoppingToken)
    {
        Logger.LogInformation(
            "Processing monitoring task for conversation {Id}: {Description}",
            task.ConversationId, task.Description);

        var workflow = MonitoringWorkflow.Build(Analyst);
        
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            workflow, 
            new MonitoringRequest(task.Description),
            cancellationToken: stoppingToken);
        
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

        await foreach (WorkflowEvent evt in run.WatchStreamAsync().WithCancellation(stoppingToken))
        {
            Console.WriteLine($"[Event] Type: {evt.GetType().Name} | Data: {evt.Data?.GetType().Name ?? "null"}");
            
            
        }
        
    }
}