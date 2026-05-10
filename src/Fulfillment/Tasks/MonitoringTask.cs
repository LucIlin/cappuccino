using Workflows.Messages;

namespace Fulfillment.Tasks;

public sealed record MonitoringTask(string ConversationId, string Description);