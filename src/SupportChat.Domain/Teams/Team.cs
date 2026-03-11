using SupportChat.Domain.Agents;

namespace SupportChat.Domain.Teams;

public class Team
{
    public Guid Id { get; }
    public string Name { get; }
    public IReadOnlyCollection<Agent> Agents { get; }

    public Team(Guid id, string name, IReadOnlyCollection<Agent> agents)
    {
        Id = id;
        Name = name;
        Agents = agents;
    }

    public int GetQueueLimit()
    {
        throw new NotImplementedException();
    }

    public int GetTotalCapacity()
    {
        throw new NotImplementedException();
    }
}