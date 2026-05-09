using System.Threading.Channels;
using Api.Tasks;
using Microsoft.Agents.AI.Workflows;

namespace Api.Workers;

public class MonitoringWorker : BackgroundService
{
    private readonly Channel<MonitoringTask> _channel;
    private readonly Workflow _workflow;

    public MonitoringWorker(
        Channel<MonitoringTask> channel,
        [FromKeyedServices("MonitoringWorkflow")] Workflow workflow)
    {
        _channel = channel;
        _workflow = workflow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in _channel.Reader.ReadAllAsync(stoppingToken))
            _ = ProcessTaskAsync(task, stoppingToken);
    }

    private async Task ProcessTaskAsync(MonitoringTask task, CancellationToken stoppingToken)
    {
        await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
            _workflow, 
            task.Request,
            cancellationToken: stoppingToken);
        
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
        
        
    }
}