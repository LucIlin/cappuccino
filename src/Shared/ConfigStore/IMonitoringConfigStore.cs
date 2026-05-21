namespace Shared.ConfigStore;

public interface IMonitoringConfigStore
{
    Task SaveAsync(ApiMonitoringConfig config, CancellationToken ct = default);
    Task<IReadOnlyList<ApiMonitoringConfig>> LoadAllAsync(CancellationToken ct = default);
}