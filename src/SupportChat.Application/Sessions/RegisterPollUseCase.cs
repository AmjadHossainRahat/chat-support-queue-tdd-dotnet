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
        var session = _chatSessionRepository.GetById(sessionId);

        if (session is null)
        {
            throw new InvalidOperationException($"Session '{sessionId}' was not found.");
        }

        session.RegisterPoll(polledAtUtc);
        _chatSessionRepository.Update(session);

        return session;
    }
}