using Shared;

namespace Api.Requests;

public sealed record SendMessageRequest(
    string Text,
    IReadOnlyList<ConversationMessage> History);