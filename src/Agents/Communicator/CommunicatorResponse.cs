using System.Text.Json.Serialization;

namespace Agents.Shared;

public sealed class CommunicatorResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } =  string.Empty;
    [JsonPropertyName("status")]
    public CommunicatorStatus Status  { get; set; } = CommunicatorStatus.None;
    [JsonPropertyName("requirements")]
    public List<Requirement> Requirements { get; set; } = [];
}