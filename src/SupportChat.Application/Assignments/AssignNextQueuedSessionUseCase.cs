using SupportChat.Application.Abstractions;
using SupportChat.Domain.Agents;
using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Assignments;

public class AssignNextQueuedSessionUseCase
{
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly AssignWaitingSessionUseCase _assignWaitingSessionUseCase;

    public AssignNextQueuedSessionUseCase(
        IChatSessionRepository chatSessionRepository,
        AssignWaitingSessionUseCase assignWaitingSessionUseCase)
    {
        _chatSessionRepository = chatSessionRepository;
        _assignWaitingSessionUseCase = assignWaitingSessionUseCase;
    }

    public ChatSession? Execute(IEnumerable<Agent> agents)
    {
        return ExecuteAsync(agents, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    public async Task<ChatSession?> ExecuteAsync(
        IEnumerable<Agent> agents,
        CancellationToken cancellationToken = default)
    {
        var nextQueuedSession = (await _chatSessionRepository.GetQueuedSessionsAsync(cancellationToken))
            .OrderBy(x => x.CreatedAtUtc)
            .FirstOrDefault();

        if (nextQueuedSession is null)
        {
            return null;
        }

        _assignWaitingSessionUseCase.Execute(nextQueuedSession, agents);
        await _chatSessionRepository.UpdateAsync(nextQueuedSession, cancellationToken);

        return nextQueuedSession;
    }
}