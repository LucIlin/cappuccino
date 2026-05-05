using Workflows.Results;

namespace Api.Tasks;

public sealed record ErrandFulfillmentTask(string ConversationId, CommunicatorResult Task);