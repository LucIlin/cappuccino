using Shared;

namespace Workflows.Messages;

public sealed record UserInput(
    string Text,
    string ConversationId,
    IReadOnlyList<ConversationMessage> History);