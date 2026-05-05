using System.Threading.Channels;
using Api.Tasks;

namespace Api.Workers;

public class MonitoringWorker : BackgroundService
{
    private readonly Channel<MonitoringTask> _channel;

    public MonitoringWorker(Channel<MonitoringTask> channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in _channel.Reader.ReadAllAsync(stoppingToken))
            _ = ProcessFulfillmentAsync(task, stoppingToken);
    }

    private async Task ProcessFulfillmentAsync(MonitoringTask task, CancellationToken stoppingToken)
    {
        Console.WriteLine($"Fulfillment task started {task.Task}");
    }
}