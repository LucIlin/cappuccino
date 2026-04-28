namespace Agents.Agents;

/// <summary>
/// Loads agent definition files from <c>AgentsDirectory</c>, which resolves to
/// <c>{AppContext.BaseDirectory}/Agents/</c> (i.e. an <c>Agents/</c> subfolder inside the
/// build output directory). Agent definitions are expected to be copied there during build —
/// see the <c>.csproj</c> glob that includes <c>**/*.yaml</c> files.
///
/// <para>
/// <see cref="Load"/> looks for <c>{agentName}.json</c> first, then <c>{agentName}.yaml</c>,
/// both resolved under <c>AgentsDirectory</c>. The first match wins and its raw text content
/// is returned. A <see cref="FileNotFoundException"/> is thrown if neither variant exists.
/// </para>
/// </summary>
public static class AgentDefinitionLoader
{
    private static readonly string AgentsDirectory =
        Path.Combine(AppContext.BaseDirectory, "Agents/Definitions");
    public static string Load(string agentName)
    {
        var jsonPath = Path.Combine(AgentsDirectory, $"{agentName}.json");
        var yamlPath = Path.Combine(AgentsDirectory, $"{agentName}.yaml");

        if (File.Exists(jsonPath))
            return File.ReadAllText(jsonPath);

        if (File.Exists(yamlPath))
            return File.ReadAllText(yamlPath);

        throw new FileNotFoundException($"No agent definition found for: {agentName}");
    }
}