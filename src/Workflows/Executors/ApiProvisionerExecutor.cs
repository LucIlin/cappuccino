using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Agents.ApiProvisioner;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Shared.ConfigStore;
using Workflows.Messages;
using Workflows.Paths;

namespace Workflows.Executors;

[YieldsOutput(typeof(ProvisioningComplete))]
internal sealed partial class ApiProvisionerExecutor(
    AIAgent agent,
    IMonitoringConfigStore configStore,
    ILogger<ApiProvisionerExecutor> logger) : Executor("ApiProvisioner")
{
    [MessageHandler]
    private async ValueTask HandleAsync(ApiMonitoringPath path, IWorkflowContext context)
    {
        AgentResponse<ApiProvisionerResponse> response = await agent.RunAsync<ApiProvisionerResponse>(
            new ChatMessage(ChatRole.User,
                $"""
                Research brief:
                URL: {path.Url}
                Context: {path.Context}
                Reasoning: {path.Reasoning}
                """));

        if (response.Result is null)
            throw new InvalidOperationException(
                "ApiProvisioner agent returned a null response.");

        var config = new ApiMonitoringConfig(
            Id:               Guid.NewGuid(),
            Endpoint:         response.Result.Endpoint,
            HttpMethod:       response.Result.HttpMethod,
            Headers:          response.Result.Headers ?? [],
            RequestBody:      response.Result.RequestBody,
            SignalPath:       response.Result.SignalPath,
            SignalCondition:  response.Result.SignalCondition,
            SignalThreshold:  response.Result.SignalThreshold,
            PollingInterval:  TimeSpan.FromSeconds(response.Result.PollingIntervalSeconds),
            CreatedAt:        DateTimeOffset.UtcNow);

        await configStore.SaveAsync(config);

        logger.LogInformation(
            "Provisioned API monitor {ConfigId} targeting {Endpoint}",
            config.Id, config.Endpoint);

        await context.YieldOutputAsync(new ProvisioningComplete(config.Id));
    }
}