namespace Worker.Settings;

public class LLMConfiguration
{
    public Dictionary<string, LLMProfile> Profiles { get; set; } = new();
}