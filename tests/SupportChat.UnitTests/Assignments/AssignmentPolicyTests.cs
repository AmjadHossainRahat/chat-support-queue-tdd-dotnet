using SupportChat.Domain.Agents;
using SupportChat.Domain.Assignments;

namespace SupportChat.UnitTests.Assignments;

public class AssignmentPolicyTests
{
    [Test]
    public void Should_assign_to_junior_before_mid()
    {
        var junior = new Agent(Guid.NewGuid(), Seniority.Junior);
        var mid = new Agent(Guid.NewGuid(), Seniority.Mid);

        var agents = new List<Agent>
        {
            mid,
            junior
        };

        var policy = new AssignmentPolicy();

        var result = policy.SelectNextAgent(agents);

        Assert.That(result, Is.EqualTo(junior));
    }

    [Test]
    public void Should_assign_to_mid_when_no_junior_available()
    {
        var mid1 = new Agent(Guid.NewGuid(), Seniority.Mid);
        var mid2 = new Agent(Guid.NewGuid(), Seniority.Mid);

        var agents = new List<Agent>
        {
            mid1,
            mid2
        };

        var policy = new AssignmentPolicy();

        var result = policy.SelectNextAgent(agents);

        Assert.That(result, Is.EqualTo(mid1));
    }

    [Test]
    public void Should_assign_to_senior_when_no_junior_or_mid_available()
    {
        var senior = new Agent(Guid.NewGuid(), Seniority.Senior);

        var agents = new List<Agent>
        {
            senior
        };

        var policy = new AssignmentPolicy();

        var result = policy.SelectNextAgent(agents);

        Assert.That(result, Is.EqualTo(senior));
    }

    [Test]
    public void Should_assign_to_team_lead_last()
    {
        var teamLead = new Agent(Guid.NewGuid(), Seniority.TeamLead);

        var agents = new List<Agent>
        {
            teamLead
        };

        var policy = new AssignmentPolicy();

        var result = policy.SelectNextAgent(agents);

        Assert.That(result, Is.EqualTo(teamLead));
    }

    [Test]
    public void Should_throw_when_no_agents_available()
    {
        var policy = new AssignmentPolicy();

        Assert.Throws<InvalidOperationException>(() =>
            policy.SelectNextAgent(new List<Agent>())
        );
    }

    [Test]
    public void Should_skip_agent_who_is_already_at_capacity()
    {
        var fullJunior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 4);
        var availableMid = new Agent(Guid.NewGuid(), Seniority.Mid, activeChatCount: 0);

        var policy = new AssignmentPolicy();

        var selected = policy.SelectNextAgent(new[] { fullJunior, availableMid });

        Assert.That(selected, Is.EqualTo(availableMid));
    }

    [Test]
    public void Should_throw_when_all_agents_are_at_capacity()
    {
        var junior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 4);
        var mid = new Agent(Guid.NewGuid(), Seniority.Mid, activeChatCount: 6);
        var senior = new Agent(Guid.NewGuid(), Seniority.Senior, activeChatCount: 8);
        var teamLead = new Agent(Guid.NewGuid(), Seniority.TeamLead, activeChatCount: 5);

        var policy = new AssignmentPolicy();

        Assert.Throws<InvalidOperationException>(() =>
            policy.SelectNextAgent(new[] { junior, mid, senior, teamLead }));
    }
}