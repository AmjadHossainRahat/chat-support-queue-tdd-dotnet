using System.Collections.Concurrent;
using SupportChat.Application.Abstractions;
using SupportChat.Domain.Sessions;

namespace SupportChat.Infrastructure.Persistence;

public class InMemoryChatSessionRepository : IChatSessionRepository
{
    private readonly ConcurrentDictionary<Guid, ChatSession> _sessions = new();

    public void Add(ChatSession session)
    {
        if (!_sessions.TryAdd(session.Id, session))
        {
            throw new InvalidOperationException($"Session '{session.Id}' already exists.");
        }
    }

    public ChatSession? GetById(Guid sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return session;
    }

    public void Update(ChatSession session)
    {
        _sessions[session.Id] = session;
    }
}