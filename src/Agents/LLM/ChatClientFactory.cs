using Microsoft.Extensions.AI;

namespace Agents.LLM;

public class ChatClientFactory
{
    private readonly Dictionary<string, Func<LLMProfile, IChatClient>> _builders = new();

    public static ChatClientFactory BuildFactory(Action<ChatClientFactory> configure)
    {
        var factory = new ChatClientFactory();
        configure(factory);
        return factory;
    }
    
    public void Register(string profileName, Func<LLMProfile, IChatClient> builder)
    {
        _builders.Add(profileName, builder);
    }

    public IChatClient Create(LLMProfile profile)
    {
        if (!_builders.TryGetValue(profile.Name, out var builder))
             throw new InvalidOperationException($"Invalid profile identifier: {profile.Name}");
        return builder(profile);
    }

}