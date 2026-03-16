using SupportChat.Application.Abstractions;
using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Sessions;

public class RegisterPollUseCase
{
    private readonly IChatSessionRepository _chatSessionRepository;

    public RegisterPollUseCase(IChatSessionRepository chatSessionRepository)
    {
        _chatSessionRepository = chatSessionRepository;
    }

    public ChatSession Execute(Guid sessionId, DateTime polledAtUtc)
    {
        return ExecuteAsync(sessionId, polledAtUtc, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    public async Task<ChatSession> ExecuteAsync(
        Guid sessionId,
        DateTime polledAtUtc,
        CancellationToken cancellationToken = default)
    {
        var session = await _chatSessionRepository.GetByIdAsync(sessionId, cancellationToken);

        if (session is null)
        {
            throw new InvalidOperationException($"Session '{sessionId}' was not found.");
        }

        session.RegisterPoll(polledAtUtc);
        await _chatSessionRepository.UpdateAsync(session, cancellationToken);

        return session;
    }
}