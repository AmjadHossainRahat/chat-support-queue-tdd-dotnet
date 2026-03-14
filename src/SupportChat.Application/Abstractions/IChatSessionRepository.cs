using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Abstractions;

public interface IChatSessionRepository
{
    void Add(ChatSession session);
    ChatSession? GetById(Guid sessionId);
    IReadOnlyCollection<ChatSession> GetQueuedSessions();
    void Update(ChatSession session);
}