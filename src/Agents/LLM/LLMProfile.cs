namespace Agents.LLM;

public class LLMProfile
{
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } =  string.Empty;
    public string Model { get; set; }  = string.Empty;
    public string ChatClient { get; set; } = string.Empty;
}