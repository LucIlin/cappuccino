using System.Text.Json.Serialization;
using Agents.Shared;

namespace Agents.Communicator;

public sealed class CommunicatorResponse
{
    [JsonPropertyName("message")]
    public string Message { get; init; } =  string.Empty;
    [JsonPropertyName("status")]
    public CommunicatorStatus Status  { get; init; } = CommunicatorStatus.None;
    [JsonPropertyName("summary")]
    public string? Summary { get; init; } = string.Empty;
}