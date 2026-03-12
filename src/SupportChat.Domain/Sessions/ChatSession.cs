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
    }

    public void RegisterPoll(DateTime polledAtUtc)
    {
        throw new NotImplementedException();
    }

    public void MarkAssigned()
    {
        throw new NotImplementedException();
    }

    public void MarkInactive()
    {
        throw new NotImplementedException();
    }

    public void MarkRejected()
    {
        throw new NotImplementedException();
    }

    public void MarkCompleted()
    {
        throw new NotImplementedException();
    }
}