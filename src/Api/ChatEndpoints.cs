using System.Text.Json;
using Api.Responses;
using Api.Requests;
using Microsoft.Agents.AI.Workflows;
using Workflows.Messages;
using Microsoft.AspNetCore.OpenApi;                                                                                                 

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
            Workflow workflow,
            HttpResponse response,
            CancellationToken ct) =>
        {
            response.Headers.ContentType = "text/event-stream";
            response.Headers.CacheControl = "no-cache";
            
            await using StreamingRun run = await InProcessExecution.RunStreamingAsync(
                workflow,
                new UserInput(request.Text, conversationId),
                cancellationToken :ct);

            await run.TrySendMessageAsync(new TurnToken(emitEvents: true));
            
            await foreach (WorkflowEvent evt in run.WatchStreamAsync().WithCancellation(ct))
            {
                Console.WriteLine($"[Event] Type: {evt.GetType().Name} | Data: {evt.Data?.GetType().Name ?? "null"}");
                
                if (evt is WorkflowOutputEvent { Data: UserFacingMessage msg })
                {
                    await response.WriteAsync($"data: {JsonSerializer.Serialize(msg.Text)}\n\n", ct);
                    await response.Body.FlushAsync(ct);
                }
            }
        }).Produces<string>(200, "text/event-stream");
        
        return routes;
    }
}