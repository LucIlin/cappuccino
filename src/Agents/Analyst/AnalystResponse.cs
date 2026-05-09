using System.Text.Json.Serialization;
using Agents.Analyst;

namespace Agents.Analyst;

public sealed class AnalystResponse
{
    [JsonPropertyName("pathType")]
    public MonitoringPathType PathType { get; init; } = MonitoringPathType.None;

    [JsonPropertyName("reasoning")]
    public string Reasoning { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("context")]
    public string? Context { get; init; }

    [JsonPropertyName("serviceName")]
    public string? ServiceName { get; init; }

    [JsonPropertyName("serviceUrl")]
    public string? ServiceUrl { get; init; }

    [JsonPropertyName("unresolvableReason")]
    public string? UnresolvableReason { get; init; }
}