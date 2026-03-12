using SupportChat.Domain.Agents;

namespace SupportChat.Domain.Assignments;

public class AssignmentPolicy
{
    public Agent SelectNextAgent(IEnumerable<Agent> agents)
    {
        var eligibleAgents = agents
            .Where(a => a.CanTakeMoreChats())
            .OrderBy(GetPriority)
            .ThenBy(a => a.ActiveChatCount)
            .ToList();

        if (eligibleAgents.Count == 0)
        {
            throw new InvalidOperationException("No eligible agents available for assignment.");
        }

        return eligibleAgents.First();
    }

    // Explicit domain rule: cannot depend on Enum value.
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