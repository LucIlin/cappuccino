using System.Text.Json;

namespace Shared.ConfigStore;

public sealed class JsonFileMonitoringConfigStore : IMonitoringConfigStore
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonFileMonitoringConfigStore(string filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveAsync(ApiMonitoringConfig config, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var existing = await LoadInternalAsync(ct);
            var updated = existing.Append(config).ToList();
            await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(updated), ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<IReadOnlyList<ApiMonitoringConfig>> LoadAllAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            return await LoadInternalAsync(ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<ApiMonitoringConfig>> LoadInternalAsync(CancellationToken ct)
    {
        if (!File.Exists(_filePath))
            return [];

        var json = await File.ReadAllTextAsync(_filePath, ct);
        return JsonSerializer.Deserialize<List<ApiMonitoringConfig>>(json) ?? [];
    }
}