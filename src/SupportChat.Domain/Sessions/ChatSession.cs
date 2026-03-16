namespace SupportChat.Domain.Sessions;

public class ChatSession
{
    public Guid Id { get; }
    public SessionStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? LastPolledAtUtc { get; private set; }
    public Guid? AssignedAgentId { get; private set; }
    public string CorrelationId { get; }

    public ChatSession(
        Guid id,
        DateTime createdAtUtc,
        string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            throw new ArgumentException("Correlation id is required.", nameof(correlationId));
        }

        Id = id;
        CreatedAtUtc = createdAtUtc;
        CorrelationId = correlationId;
        Status = SessionStatus.Queued;
    }

    public void RegisterPoll(DateTime polledAtUtc)
    {
        if (polledAtUtc < CreatedAtUtc)
        {
            throw new ArgumentException("Poll time cannot be earlier than session creation time.", nameof(polledAtUtc));
        }

        LastPolledAtUtc = polledAtUtc;
    }

    public void AssignTo(Guid agentId)
    {
        if (agentId == Guid.Empty)
        {
            throw new ArgumentException("Assigned agent id cannot be empty.", nameof(agentId));
        }

        if (Status != SessionStatus.Queued)
        {
            throw new InvalidOperationException($"Cannot assign a session in {Status} state.");
        }

        Status = SessionStatus.Assigned;
        AssignedAgentId = agentId;
    }

    public void MarkInactive()
    {
        Status = SessionStatus.Inactive;
    }

    public void MarkCompleted()
    {
        if (Status != SessionStatus.Assigned)
        {
            throw new InvalidOperationException("Only an assigned session can be completed.");
        }

        Status = SessionStatus.Completed;
    }

    public void MarkRejected()
    {
        if (Status is SessionStatus.Assigned or SessionStatus.Completed)
        {
            throw new InvalidOperationException($"Cannot reject a session in {Status} state.");
        }

        Status = SessionStatus.Rejected;
    }

    public static ChatSession Rehydrate(
        Guid id,
        DateTime createdAtUtc,
        SessionStatus status,
        DateTime? lastPolledAtUtc,
        Guid? assignedAgentId,
        string correlationId)
    {
        var session = new ChatSession(id, createdAtUtc, correlationId)
        {
            Status = status,
            LastPolledAtUtc = lastPolledAtUtc,
            AssignedAgentId = assignedAgentId
        };

        return session;
    }
}