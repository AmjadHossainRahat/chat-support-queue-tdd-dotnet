using SupportChat.Domain.Agents;
using SupportChat.Domain.Teams;

namespace SupportChat.API.Providers;

public static class TeamProvider
{
    public static Team CreateMainTeam()
    {
        var agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.TeamLead),
            new(Guid.NewGuid(), Seniority.Mid),
            new(Guid.NewGuid(), Seniority.Mid),
            new(Guid.NewGuid(), Seniority.Junior)
        };

        return new Team(Guid.NewGuid(), "Team A", agents);
    }

    public static Team CreateOverflowTeam()
    {
        var agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior)
        };

        return new Team(Guid.NewGuid(), "Overflow", agents);
    }
}