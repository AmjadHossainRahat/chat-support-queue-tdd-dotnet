using SupportChat.Domain.Agents;
using SupportChat.Domain.Teams;

namespace SupportChat.UnitTests.Teams;

public class TeamCapacityTests
{
    [Test]
    public void Team_capacity_should_be_sum_of_all_agent_capacities()
    {
        var agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.TeamLead), // 5
            new(Guid.NewGuid(), Seniority.Mid),      // 6
            new(Guid.NewGuid(), Seniority.Mid),      // 6
            new(Guid.NewGuid(), Seniority.Junior)    // 4
        };

        var team = new Team(Guid.NewGuid(), "Team A", agents);

        var capacity = team.GetTotalCapacity();

        Assert.That(capacity, Is.EqualTo(21));
    }

    [Test]
    public void Team_queue_limit_should_be_floor_of_capacity_times_one_point_five()
    {
        var agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.Mid),      // 6
            new(Guid.NewGuid(), Seniority.Mid),      // 6
            new(Guid.NewGuid(), Seniority.Junior)    // 4
        };

        var team = new Team(Guid.NewGuid(), "Sample Team", agents);

        var queueLimit = team.GetQueueLimit();

        Assert.That(queueLimit, Is.EqualTo(24));
    }

    [Test]
    public void Team_with_no_agents_should_have_zero_capacity()
    {
        var team = new Team(Guid.NewGuid(), "Empty Team", new List<Agent>());

        var capacity = team.GetTotalCapacity();

        Assert.That(capacity, Is.EqualTo(0));
    }

    [Test]
    public void Queue_limit_should_be_zero_when_team_has_no_agents()
    {
        var team = new Team(Guid.NewGuid(), "Empty Team", new List<Agent>());

        var queueLimit = team.GetQueueLimit();

        Assert.That(queueLimit, Is.EqualTo(0));
    }
}