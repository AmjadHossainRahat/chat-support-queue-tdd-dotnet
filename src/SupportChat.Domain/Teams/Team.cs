using SupportChat.Domain.Agents;

namespace SupportChat.Domain.Teams;

public class Team
{
    private const double QueueLimitMultiplier = 1.5;

    public Guid Id { get; }
    public string Name { get; }
    public IReadOnlyCollection<Agent> Agents { get; }

    public Team(Guid id, string name, IReadOnlyCollection<Agent> agents)
    {
        Id = id;
        Name = name;
        Agents = agents;
    }

    public int GetTotalCapacity()
    {
        return Agents.Sum(agent => agent.GetMaxConcurrentChats());
    }

    public int GetQueueLimit()
    {
        return (int)Math.Floor(GetTotalCapacity() * QueueLimitMultiplier);
    }
}