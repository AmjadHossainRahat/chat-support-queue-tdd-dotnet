using SupportChat.Application.Abstractions;
using SupportChat.Domain.Queues;
using SupportChat.Domain.Sessions;
using SupportChat.Domain.Teams;

namespace SupportChat.Application.Sessions;

public class CreateChatSessionUseCase
{
    private readonly QueueAdmissionPolicy _queueAdmissionPolicy;
    private readonly IChatSessionRepository _chatSessionRepository;

    public CreateChatSessionUseCase(
        QueueAdmissionPolicy queueAdmissionPolicy,
        IChatSessionRepository chatSessionRepository)
    {
        _queueAdmissionPolicy = queueAdmissionPolicy;
        _chatSessionRepository = chatSessionRepository;
    }

    public CreateChatSessionResult Execute(
        Team mainTeam,
        int currentMainQueueCount,
        Team overflowTeam,
        int currentOverflowQueueCount,
        DateTime nowUtc)
    {
        var admissionResult = _queueAdmissionPolicy.Decide(
            mainTeam,
            currentMainQueueCount,
            overflowTeam,
            currentOverflowQueueCount,
            TimeOnly.FromDateTime(nowUtc));

        if (admissionResult == QueueAdmissionResult.Rejected)
        {
            return new CreateChatSessionResult(
                AdmissionResult: QueueAdmissionResult.Rejected,
                SessionId: null);
        }

        var session = new ChatSession(
            id: Guid.NewGuid(),
            createdAtUtc: nowUtc);

        _chatSessionRepository.Add(session);

        return new CreateChatSessionResult(
            AdmissionResult: admissionResult,
            SessionId: session.Id);
    }
}