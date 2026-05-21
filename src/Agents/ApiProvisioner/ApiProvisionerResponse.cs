using System.Text.Json.Serialization;

namespace Agents.ApiProvisioner;

public sealed class ApiProvisionerResponse
{
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; init; } = string.Empty;

    [JsonPropertyName("httpMethod")]
    public string HttpMethod { get; init; } = string.Empty;

    [JsonPropertyName("headers")]
    public Dictionary<string, string>? Headers { get; init; }

    [JsonPropertyName("requestBody")]
    public string? RequestBody { get; init; }

    [JsonPropertyName("signalPath")]
    public string SignalPath { get; init; } = string.Empty;

    [JsonPropertyName("signalCondition")]
    public string SignalCondition { get; init; } = string.Empty;

    [JsonPropertyName("signalThreshold")]
    public string? SignalThreshold { get; init; }

    [JsonPropertyName("pollingIntervalSeconds")]
    public int PollingIntervalSeconds { get; init; }

    [JsonPropertyName("reasoning")]
    public string Reasoning { get; init; } = string.Empty;
}