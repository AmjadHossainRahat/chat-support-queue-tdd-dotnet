using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Abstractions;

public interface IChatSessionRepository
{
    void Add(ChatSession session);
    ChatSession? GetById(Guid sessionId);
    IReadOnlyCollection<ChatSession> GetQueuedSessions();
    void Update(ChatSession session);

    Task AddAsync(ChatSession session, CancellationToken cancellationToken = default)
    {
        Add(session);
        return Task.CompletedTask;
    }

    Task<ChatSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetById(sessionId));
    }

    Task<IReadOnlyCollection<ChatSession>> GetQueuedSessionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GetQueuedSessions());
    }

    Task UpdateAsync(ChatSession session, CancellationToken cancellationToken = default)
    {
        Update(session);
        return Task.CompletedTask;
    }
}