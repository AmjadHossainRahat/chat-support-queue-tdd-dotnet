using Microsoft.EntityFrameworkCore;
using SupportChat.Application.Abstractions;
using SupportChat.Domain.Sessions;

namespace SupportChat.Infrastructure.Persistence;

public class SqliteChatSessionRepository : IChatSessionRepository
{
    private readonly SupportChatDbContext _dbContext;

    public SqliteChatSessionRepository(SupportChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ChatSession session)
    {
        var record = ToRecord(session);

        _dbContext.ChatSessions.Add(record);
        _dbContext.SaveChanges();
    }

    public ChatSession? GetById(Guid sessionId)
    {
        var record = _dbContext.ChatSessions
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == sessionId);

        return record is null ? null : ToDomain(record);
    }

    public IReadOnlyCollection<ChatSession> GetQueuedSessions()
    {
        return _dbContext.ChatSessions
            .AsNoTracking()
            .Where(x => x.Status == SessionStatus.Queued)
            .OrderBy(x => x.CreatedAtUtc)
            .ToList()
            .Select(ToDomain)
            .ToList();
    }

    public void Update(ChatSession session)
    {
        var existing = _dbContext.ChatSessions.SingleOrDefault(x => x.Id == session.Id);

        if (existing is null)
        {
            throw new InvalidOperationException($"Session '{session.Id}' was not found.");
        }

        existing.Status = session.Status;
        existing.CreatedAtUtc = session.CreatedAtUtc;
        existing.LastPolledAtUtc = session.LastPolledAtUtc;
        existing.AssignedAgentId = session.AssignedAgentId;

        _dbContext.SaveChanges();
    }

    private static ChatSessionRecord ToRecord(ChatSession session)
    {
        return new ChatSessionRecord
        {
            Id = session.Id,
            Status = session.Status,
            CreatedAtUtc = session.CreatedAtUtc,
            LastPolledAtUtc = session.LastPolledAtUtc,
            AssignedAgentId = session.AssignedAgentId
        };
    }

    private static ChatSession ToDomain(ChatSessionRecord record)
    {
        return ChatSession.Rehydrate(
            record.Id,
            record.CreatedAtUtc,
            record.Status,
            record.LastPolledAtUtc,
            record.AssignedAgentId);
    }
}