using SupportChat.Domain.Sessions;

namespace SupportChat.Infrastructure.Persistence;

public class ChatSessionRecord
{
    public Guid Id { get; set; }
    public SessionStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastPolledAtUtc { get; set; }
    public Guid? AssignedAgentId { get; set; }
}