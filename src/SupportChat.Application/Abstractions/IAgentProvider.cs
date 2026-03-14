using SupportChat.Domain.Agents;

namespace SupportChat.Application.Abstractions;

public interface IAgentProvider
{
    IReadOnlyCollection<Agent> GetAvailableAgents();
}