using SupportChat.Domain.Agents;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Sessions;

namespace SupportChat.Application.Assignments;

public class AssignWaitingSessionUseCase
{
    private readonly AssignmentPolicy _assignmentPolicy;

    public AssignWaitingSessionUseCase(AssignmentPolicy assignmentPolicy)
    {
        _assignmentPolicy = assignmentPolicy;
    }

    public Agent Execute(ChatSession session, IEnumerable<Agent> agents)
    {
        throw new NotImplementedException();
    }
}