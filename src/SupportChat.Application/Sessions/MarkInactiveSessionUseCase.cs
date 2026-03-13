using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Sessions;

public class MarkInactiveSessionUseCase
{
    private readonly SessionActivityPolicy _sessionActivityPolicy;

    public MarkInactiveSessionUseCase(SessionActivityPolicy sessionActivityPolicy)
    {
        _sessionActivityPolicy = sessionActivityPolicy;
    }

    public void Execute(ChatSession session, DateTime nowUtc)
    {
        throw new NotImplementedException();
    }
}