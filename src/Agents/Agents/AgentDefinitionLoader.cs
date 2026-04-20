namespace Agents.Agents;

public static class AgentDefinitionLoader
{
    public static string Load(string agentName)
    {
        var jsonPath = $"Agents/{agentName}.json";
        var yamlPath = $"Agents/{agentName}.yaml";

        if (File.Exists(jsonPath))
            return File.ReadAllText(jsonPath);

        if (File.Exists(yamlPath))
            return File.ReadAllText(yamlPath);

        throw new FileNotFoundException($"No agent definition found for: {agentName}");
    }
}