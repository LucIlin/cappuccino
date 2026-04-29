# Setup

# Agents
### Extractor
- Interpreter of userprompt.
- output schema is dynamic.
- reads intent and registers entities.

## Communicator Agent

### Role

The Communicator is the only agent in the system that talks directly to the user. It works with the user to formulate a plan for a real world event they want to be notified of, gating the conversation until the user has provided enough information and explicitly confirmed the plan.

### How It Works

The Communicator runs once per user message. On each turn it:

1. Reads the current user message in the context of the conversation history, which is provided automatically by `AgentSession`.
2. Classifies the message and sets its `status`.
3. Replies conversationally to the user, asking one focused follow-up question if more detail would be helpful.

The Communicator does not store, read, or render the structured details list. Its awareness that details are being gathered comes only from the conversational history in `AgentSession`.

### Status Values

The status reflects the content of the current user message, not whether the conversation feels complete.

| Status | Meaning |
|---|---|
| `HasDetails` | The current message contains actionable information about the event (subject, location, timing, condition, etc.) |
| `Eliciting` | The current message contains no actionable information (greetings, off-topic chitchat, vague replies, questions back to the agent) |
| `Confirmed` | The user has clearly agreed the plan is complete in response to an explicit confirmation question |

`HasDetails` can be set while the agent is still asking a follow-up question. The two are not mutually exclusive.

### Scope

The Communicator only asks about the event itself: what it is, who or what it concerns, where it occurs, when it occurs, and any conditions that should trigger the notification. It explicitly does not ask about delivery method (email, SMS, push), notification frequency, or any system or account preferences. Those concerns belong to other parts of the system.

### Tone

The Communicator acknowledges without committing. Reflective phrases like "noted" or "got it, so far we have" are preferred over commitment phrases like "I'll notify you" — the plan is not set up until the user confirms.

### Output Contract

```json
{
  "message": "<conversational reply to user>",
  "status": "HasDetails | Eliciting | Confirmed"
}
```

### Routing

The Communicator is routing-agnostic. It only sets its `status` — the `WorkflowBuilder` decides where the next message goes based on that value. The Communicator has no knowledge of downstream agents.

### Definition

Defined declaratively in `Agents/Definitions/communicator.yaml`, loaded via `ChatClientPromptAgentFactory`.

### YAML Authoring Notes

The MAF YAML parser has two undocumented constraints that affect this definition:
- Multiline strings must use the literal block scalar `|`. The folded scalar `>` causes parse failures.
- Em dashes and other non-ASCII punctuation should be avoided in string values to prevent parser issues.