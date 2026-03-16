using SupportChat.Application.Abstractions;
using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Sessions;

public class GetChatSessionByIdUseCase
{
    private readonly IChatSessionRepository _chatSessionRepository;

    public GetChatSessionByIdUseCase(IChatSessionRepository chatSessionRepository)
    {
        _chatSessionRepository = chatSessionRepository;
    }

    public ChatSession? Execute(Guid sessionId)
    {
        return ExecuteAsync(sessionId, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    public Task<ChatSession?> ExecuteAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return _chatSessionRepository.GetByIdAsync(sessionId, cancellationToken);
    }
}