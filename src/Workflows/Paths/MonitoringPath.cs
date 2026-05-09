namespace Workflows.Paths;

public abstract record MonitoringPath(string Reasoning);

public sealed record ApiMonitoringPath(
    string Url,
    string Context,
    string Reasoning) : MonitoringPath(Reasoning);

public sealed record BrowserMonitoringPath(
    string Url,
    string Context,
    string Reasoning) : MonitoringPath(Reasoning);

public sealed record ThirdPartyMonitoringPath(
    string ServiceName,
    string ServiceUrl,
    string Context,
    string Reasoning) : MonitoringPath(Reasoning);

public sealed record UnresolvableMonitoringPath(
    string Reason,
    string Reasoning) : MonitoringPath(Reasoning);