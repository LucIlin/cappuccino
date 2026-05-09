using Workflows.Messages;

namespace Api.Tasks;

public sealed record MonitoringTask(string ConversationId, MonitoringRequest Request);