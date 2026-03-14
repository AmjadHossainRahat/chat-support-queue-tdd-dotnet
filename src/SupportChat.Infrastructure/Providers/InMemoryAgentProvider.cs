using SupportChat.Application.Abstractions;
using SupportChat.Domain.Agents;

namespace SupportChat.Infrastructure.Providers;

public class InMemoryAgentProvider : IAgentProvider
{
    private readonly IReadOnlyCollection<Agent> _agents;

    public InMemoryAgentProvider()
    {
        _agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.TeamLead, activeChatCount: 0),
            new(Guid.NewGuid(), Seniority.Mid, activeChatCount: 0),
            new(Guid.NewGuid(), Seniority.Mid, activeChatCount: 0),
            new(Guid.NewGuid(), Seniority.Junior, activeChatCount: 0)
        };
    }

    public IReadOnlyCollection<Agent> GetAvailableAgents()
    {
        return _agents;
    }
}