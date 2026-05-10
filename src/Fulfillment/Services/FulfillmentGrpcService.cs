using System.Threading.Channels;
using Fulfillment.Tasks;
using Grpc.Core;
using Shared.Grpc;
using Workflows.Messages;

namespace Fulfillment.Services;

public sealed class FulfillmentGrpcService(
    Channel<MonitoringTask> channel,
    ILogger<FulfillmentGrpcService> logger)
    : FulfillmentService.FulfillmentServiceBase
{
    public override Task<SubmitMonitoringReply> SubmitMonitoringRequest(
        SubmitMonitoringRequestMessage request,
        ServerCallContext context)
    {
        var task = new MonitoringTask(request.ConversationId, request.Description);
        var accepted = channel.Writer.TryWrite(task);

        if (accepted)
        {
            logger.LogWarning(
                "Monitoring channel rejected task for conversation {Id}",
                request.ConversationId);
        }
        
        return Task.FromResult(new SubmitMonitoringReply { Accepted = accepted });
    }
}