using System.Text.Json.Serialization;

namespace Agents.Shared;

public sealed class Requirement
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; init; } =  string.Empty;
}