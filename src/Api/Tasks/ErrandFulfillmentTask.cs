using Workflows.Messages;

namespace Api.Tasks;

public sealed record ErrandFulfillmentTask(string ConversationId, MonitoringRequest Task);