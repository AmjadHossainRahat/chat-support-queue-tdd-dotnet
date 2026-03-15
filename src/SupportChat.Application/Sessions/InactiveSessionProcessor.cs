using SupportChat.Application.Abstractions;

namespace SupportChat.Application.Sessions;

public class InactiveSessionProcessor
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly MarkInactiveSessionUseCase _markInactiveSessionUseCase;

    public InactiveSessionProcessor(
        IChatSessionRepository chatSessionRepository,
        MarkInactiveSessionUseCase markInactiveSessionUseCase)
    {
        _chatSessionRepository = chatSessionRepository;
        _markInactiveSessionUseCase = markInactiveSessionUseCase;
    }

    public int Execute(DateTime nowUtc)
    {
        var processedCount = 0;

        var queuedSessions = _chatSessionRepository.GetQueuedSessions();

        foreach (var session in queuedSessions)
        {
            var previousStatus = session.Status;

            _markInactiveSessionUseCase.Execute(session, nowUtc);

            if (session.Status != previousStatus)
            {
                _chatSessionRepository.Update(session);
                processedCount++;
            }
        }

        return processedCount;
    }
}