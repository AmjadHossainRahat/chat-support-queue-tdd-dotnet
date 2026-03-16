using SupportChat.Application.Abstractions;
using SupportChat.Domain.Sessions;

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
        return ExecuteAsync(nowUtc, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    public async Task<int> ExecuteAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        var processedCount = 0;

        var queuedSessions = await _chatSessionRepository.GetQueuedSessionsAsync(cancellationToken);

        foreach (var session in queuedSessions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var previousStatus = session.Status;

            _markInactiveSessionUseCase.Execute(session, nowUtc);

            if (session.Status != previousStatus)
            {
                await _chatSessionRepository.UpdateAsync(session, cancellationToken);
                processedCount++;
            }
        }

        return processedCount;
    }
}