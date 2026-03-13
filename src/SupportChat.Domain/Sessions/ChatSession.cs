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
        if (Status is SessionStatus.Inactive or SessionStatus.Rejected or SessionStatus.Completed)
        {
            throw new InvalidOperationException($"Cannot assign a session in {Status} state.");
        }

        Status = SessionStatus.Assigned;
    }

    public void MarkInactive()
    {
        if (Status is SessionStatus.Rejected or SessionStatus.Completed)
        {
            throw new InvalidOperationException($"Cannot mark a session in {Status} state as inactive.");
        }

        Status = SessionStatus.Inactive;
    }

    public void MarkRejected()
    {
        if (Status is SessionStatus.Assigned or SessionStatus.Completed)
        {
            throw new InvalidOperationException($"Cannot reject a session in {Status} state.");
        }

        Status = SessionStatus.Rejected;
    }

    public void MarkCompleted()
    {
        if (Status != SessionStatus.Assigned)
        {
            throw new InvalidOperationException("Only an assigned session can be completed.");
        }

        Status = SessionStatus.Completed;
    }
}