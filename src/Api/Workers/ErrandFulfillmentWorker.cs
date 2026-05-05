using System.Threading.Channels;
using Api.Tasks;

namespace Api.Workers;

public class ErrandFulfillmentWorker : BackgroundService
{
    private readonly Channel<ErrandFulfillmentTask> _channel;

    public ErrandFulfillmentWorker(Channel<ErrandFulfillmentTask> channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in _channel.Reader.ReadAllAsync(stoppingToken))
            _ = ProcessFulfillmentAsync(task, stoppingToken);
    }

    private async Task ProcessFulfillmentAsync(ErrandFulfillmentTask task, CancellationToken stoppingToken)
    {
        Console.WriteLine($"Fulfillment task started {task.Task}");
    }
}