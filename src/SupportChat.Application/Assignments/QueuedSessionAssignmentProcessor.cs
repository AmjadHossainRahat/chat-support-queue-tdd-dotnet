using SupportChat.Application.Abstractions;
using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Assignments;

public class QueuedSessionAssignmentProcessor
{
    private readonly IAgentProvider _agentProvider;
    private readonly AssignNextQueuedSessionUseCase _assignNextQueuedSessionUseCase;

    public QueuedSessionAssignmentProcessor(
        IAgentProvider agentProvider,
        AssignNextQueuedSessionUseCase assignNextQueuedSessionUseCase)
    {
        _agentProvider = agentProvider;
        _assignNextQueuedSessionUseCase = assignNextQueuedSessionUseCase;
    }

    public ChatSession? Execute()
    {
        var agents = _agentProvider.GetAvailableAgents();
        return _assignNextQueuedSessionUseCase.Execute(agents);
    }
}