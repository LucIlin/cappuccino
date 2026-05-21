namespace Shared.ConfigStore;

public record ApiMonitoringConfig(
    Guid Id,
    string Endpoint,
    string HttpMethod,           
    Dictionary<string, string> Headers,
    string? RequestBody,
    string SignalPath,           // JSONPath expression e.g. "$.data.price"
    string SignalCondition,      // e.g. "greater_than", "changed", "equals"
    string? SignalThreshold,     // e.g. "150.00" — null for "changed"
    TimeSpan PollingInterval,
    DateTimeOffset CreatedAt
);