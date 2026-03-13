using SupportChat.Domain.Queues;
using SupportChat.Domain.Sessions;
using SupportChat.Domain.Teams;

namespace SupportChat.Application.Sessions;

public class CreateChatSessionUseCase
{
    private readonly QueueAdmissionPolicy _admissionPolicy;

    public CreateChatSessionUseCase(QueueAdmissionPolicy admissionPolicy)
    {
        _admissionPolicy = admissionPolicy;
    }

    public CreateChatSessionResult Execute(
        Team mainTeam,
        int currentMainQueueCount,
        Team overflowTeam,
        int currentOverflowQueueCount,
        DateTime nowUtc)
    {
        throw new NotImplementedException();
    }
}