namespace Agents.Communicator;

public enum CommunicatorStatus
{
    None,
    HasDetails, // Has details for the extractor to extract
    Eliciting, // Still gathering the initial request, message dud
    Confirmed // User confirmed — ready to send downstream
}