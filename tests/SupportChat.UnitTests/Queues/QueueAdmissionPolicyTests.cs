using SupportChat.Domain.Agents;
using SupportChat.Domain.Queues;
using SupportChat.Domain.Teams;

namespace SupportChat.UnitTests.Queues;

public class QueueAdmissionPolicyTests
{
    private static readonly OfficeHoursPolicy OfficeHours = new(new TimeOnly(9, 0), new TimeOnly(17, 0));

    [Test]
    public void Should_admit_to_main_queue_when_main_queue_has_capacity()
    {
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();
        var policy = new QueueAdmissionPolicy(OfficeHours);

        var result = policy.Decide(
            mainTeam,
            currentMainQueueCount: 10,
            overflowTeam,
            currentOverflowQueueCount: 0,
            currentTime: new TimeOnly(10, 0));

        Assert.That(result, Is.EqualTo(QueueAdmissionResult.MainQueue));
    }

    [Test]
    public void Should_route_to_overflow_when_main_queue_is_full_and_within_office_hours_and_overflow_has_capacity()
    {
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();
        var policy = new QueueAdmissionPolicy(OfficeHours);

        var result = policy.Decide(
            mainTeam,
            currentMainQueueCount: mainTeam.GetQueueLimit(),
            overflowTeam,
            currentOverflowQueueCount: 10,
            currentTime: new TimeOnly(11, 0));

        Assert.That(result, Is.EqualTo(QueueAdmissionResult.OverflowQueue));
    }

    [Test]
    public void Should_reject_when_main_queue_is_full_and_outside_office_hours()
    {
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();
        var policy = new QueueAdmissionPolicy(OfficeHours);

        var result = policy.Decide(
            mainTeam,
            currentMainQueueCount: mainTeam.GetQueueLimit(),
            overflowTeam,
            currentOverflowQueueCount: 0,
            currentTime: new TimeOnly(20, 0));

        Assert.That(result, Is.EqualTo(QueueAdmissionResult.Rejected));
    }

    [Test]
    public void Should_reject_when_main_queue_is_full_and_overflow_is_also_full()
    {
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();
        var policy = new QueueAdmissionPolicy(OfficeHours);

        var result = policy.Decide(
            mainTeam,
            currentMainQueueCount: mainTeam.GetQueueLimit(),
            overflowTeam,
            currentOverflowQueueCount: overflowTeam.GetQueueLimit(),
            currentTime: new TimeOnly(10, 0));

        Assert.That(result, Is.EqualTo(QueueAdmissionResult.Rejected));
    }

    [Test]
    public void Should_still_admit_to_main_queue_when_main_queue_has_capacity_even_within_office_hours()
    {
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();
        var policy = new QueueAdmissionPolicy(OfficeHours);

        var result = policy.Decide(
            mainTeam,
            currentMainQueueCount: mainTeam.GetQueueLimit() - 1,
            overflowTeam,
            currentOverflowQueueCount: 0,
            currentTime: new TimeOnly(16, 30));

        Assert.That(result, Is.EqualTo(QueueAdmissionResult.MainQueue));
    }

    private static Team CreateTeamA()
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

    private static Team CreateOverflowTeam()
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