using Agents.Analyst;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Workflows.Messages;
using Workflows.Paths;

namespace Workflows.Executors;

[SendsMessage(typeof(ApiMonitoringPath))]
[SendsMessage(typeof(BrowserMonitoringPath))]
[SendsMessage(typeof(ThirdPartyMonitoringPath))]
[YieldsOutput(typeof(UnresolvableMonitoringPath))]
public sealed partial class AnalystExecutor(AIAgent agent) : Executor("Analyst")
{
    [MessageHandler]
    private async ValueTask HandleAsync(MonitoringRequest request, IWorkflowContext context)
    {
        AgentResponse<AnalystResponse> response =
            await agent.RunAsync<AnalystResponse>(request.Description);

        if (response.Result.PathType == MonitoringPathType.None)
            throw new InvalidOperationException("Analyst returned a response with no path type set");

        MonitoringPath path = response.Result.PathType switch
        {
            MonitoringPathType.Api => new ApiMonitoringPath(
                response.Result.Url
                ?? throw new InvalidOperationException("Api path missing Url"),
                response.Result.Context
                ?? throw new InvalidOperationException("Api path missing Context"),
                response.Result.Reasoning),

            MonitoringPathType.Browser => new BrowserMonitoringPath(
                response.Result.Url
                ?? throw new InvalidOperationException("Browser path missing Url"),
                response.Result.Context
                ?? throw new InvalidOperationException("Browser path missing Context"),
                response.Result.Reasoning),

            MonitoringPathType.ThirdParty => new ThirdPartyMonitoringPath(
                response.Result.ServiceName
                ?? throw new InvalidOperationException("ThirdParty path missing ServiceName"),
                response.Result.ServiceUrl
                ?? throw new InvalidOperationException("ThirdParty path missing ServiceUrl"),
                response.Result.Context
                ?? throw new InvalidOperationException("ThirdParty path missing Context"),
                response.Result.Reasoning),

            MonitoringPathType.Unresolvable => new UnresolvableMonitoringPath(
                response.Result.UnresolvableReason
                ?? throw new InvalidOperationException("Unresolvable path missing Reason"),
                response.Result.Reasoning),

            _ => throw new InvalidOperationException(
                $"Analyst returned unknown path type: {response.Result.PathType}")
        };

        if (path is UnresolvableMonitoringPath)
            await context.YieldOutputAsync(path);
        else
            await context.SendMessageAsync(path);
    }
}