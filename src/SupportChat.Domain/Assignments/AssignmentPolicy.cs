using SupportChat.Domain.Agents;

namespace SupportChat.Domain.Assignments;

public class AssignmentPolicy
{
    public Agent SelectNextAgent(IEnumerable<Agent> agents)
    {
        var availableAgents = agents.ToList();

        if (availableAgents.Count == 0)
        {
            throw new InvalidOperationException("No agents available for assignment.");
        }

        return availableAgents
            //.OrderBy(a => a.Seniority)    // do not rely on enum ordering for business rules
            .OrderBy(GetPriority)
            .First();
    }

    private static int GetPriority(Agent agent)
    {
        return agent.Seniority switch
        {
            Seniority.Junior => 1,
            Seniority.Mid => 2,
            Seniority.Senior => 3,
            Seniority.TeamLead => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(agent.Seniority), agent.Seniority, null)
        };
    }
}