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
        var nextQueuedSession = _chatSessionRepository
            .GetQueuedSessions()
            .OrderBy(x => x.CreatedAtUtc)
            .FirstOrDefault();

        if (nextQueuedSession is null)
        {
            return null;
        }

        _assignWaitingSessionUseCase.Execute(nextQueuedSession, agents);
        _chatSessionRepository.Update(nextQueuedSession);

        return nextQueuedSession;
    }
}