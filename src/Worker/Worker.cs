using Microsoft.Extensions.Options;
using Worker.Settings;

namespace Worker;

public class Worker(ILogger<Worker> logger, IOptions<LLMConfiguration> _llmSettings) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Console.WriteLine(
                    $"LLM Settings: \n" +
                    $"\n\tProvider: {_llmSettings.Value.Profiles["TestProfile"].Provider}" +
                    $"\n\tApiKey: {_llmSettings.Value.Profiles["TestProfile"].ApiKey}" +
                    $"\n\tModel: {_llmSettings.Value.Profiles["TestProfile"].Model}"
                    );
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}