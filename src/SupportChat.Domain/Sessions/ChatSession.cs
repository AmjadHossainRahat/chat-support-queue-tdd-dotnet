namespace SupportChat.Domain.Sessions;

public class ChatSession
{
    public Guid Id { get; }
    public SessionStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? LastPolledAtUtc { get; private set; }

    public ChatSession(Guid id, DateTime createdAtUtc)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        Status = SessionStatus.Queued;
    }

    public void RegisterPoll(DateTime polledAtUtc)
    {
        if (polledAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException("Poll time cannot be earlier than session creation time.");
        }

        LastPolledAtUtc = polledAtUtc;
    }

    public void MarkAssigned()
    {
        Status = SessionStatus.Assigned;
    }

    public void MarkInactive()
    {
        Status = SessionStatus.Inactive;
    }

    public void MarkRejected()
    {
        Status = SessionStatus.Rejected;
    }

    public void MarkCompleted()
    {
        Status = SessionStatus.Completed;
    }
}