namespace Agents.LLM;

public class LLMConfiguration
{
    public Dictionary<string, LLMProfile> Profiles { get; set; } = new();

    public LLMProfile GetProfile(string profileName)
    {
        if (!Profiles.TryGetValue(profileName, out var profile))
            throw new KeyNotFoundException($"LLM profile: {profileName} not found. Available profiles: {string.Join(", ", Profiles.Keys)}");
        return profile;
    }
}