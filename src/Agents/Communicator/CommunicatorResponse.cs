using System.Text.Json.Serialization;
using Agents.Shared;

namespace Agents.Communicator;

public sealed class CommunicatorResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } =  string.Empty;
    [JsonPropertyName("status")]
    public CommunicatorStatus Status  { get; set; } = CommunicatorStatus.None;
}