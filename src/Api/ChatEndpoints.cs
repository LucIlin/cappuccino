using System.Text.Json;
using System.Threading.Channels;
using Api.Responses;
using Api.Requests;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Shared.Grpc;
using Workflows;
using Workflows.Messages;

namespace Api;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/conversations", () =>
            new ConversationCreatedResponse(Guid.NewGuid().ToString()));

        routes.MapPost("/conversations/{conversationId}/messages", async (
            string conversationId,
            SendMessageRequest request,
            [FromKeyedServices("Communicator")] AIAgent communicator,
            FulfillmentService.FulfillmentServiceClient fulfillmentClient,
            HttpResponse response,
            CancellationToken ct) =>
        {
            response.Headers.ContentType = "text/event-stream";
            response.Headers.CacheControl = "no-cache";
            
            var workflow = CommunicationWorkflow.Build(communicator);
            
            await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
                workflow,
                new UserInput(request.Text, conversationId, request.History),
                cancellationToken :ct);

            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            
            await foreach (WorkflowEvent evt in run.WatchStreamAsync().WithCancellation(ct))
            {
                Console.WriteLine($"[Event] Type: {evt.GetType().Name} | Data: {evt.Data?.GetType().Name ?? "null"}");

                switch (evt)
                {
                    case WorkflowOutputEvent { Data: UserFacingMessage msg }:
                        await response.WriteAsync($"data: {JsonSerializer.Serialize(msg.Text)}\n\n", ct);
                        await response.Body.FlushAsync(ct);
                        break;
                    case WorkflowOutputEvent { Data: MonitoringRequest result}:
                        await fulfillmentClient.SubmitMonitoringRequestAsync(
                            new SubmitMonitoringRequestMessage
                            {
                                ConversationId = conversationId,
                                Description = result.Description,
                            },
                            cancellationToken: ct);
                        break;
                        
                }
            }
        }).Produces<string>(200, "text/event-stream");
        
        return routes;
    }
}