using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Abstractions;

public interface IChatSessionRepository
{
    void Add(ChatSession session);
    ChatSession? GetById(Guid sessionId);
    void Update(ChatSession session);
}