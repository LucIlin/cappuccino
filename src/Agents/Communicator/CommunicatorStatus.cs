namespace Agents.Communicator;

public enum CommunicatorStatus
{
    None,
    Eliciting, // Still gathering the initial request
    Refining, // List exists, iterating with user
    AwaitingConfirmation, // Communicator has asked explicit yes/no
    Confirmed // User confirmed — ready to send downstream
}