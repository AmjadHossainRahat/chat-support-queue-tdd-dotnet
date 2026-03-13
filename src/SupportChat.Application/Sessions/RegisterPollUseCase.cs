using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Sessions;

public class RegisterPollUseCase
{
    public void Execute(ChatSession session, DateTime polledAtUtc)
    {
        session.RegisterPoll(polledAtUtc);
    }
}