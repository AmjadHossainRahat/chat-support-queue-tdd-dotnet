using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class MarkInactiveSessionUseCaseTests
{
    [Test]
    public void Should_mark_session_inactive_when_activity_policy_says_it_is_inactive()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        session.RegisterPoll(new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var activityPolicy = new SessionActivityPolicy(
            expectedPollInterval: TimeSpan.FromSeconds(1),
            missedPollThreshold: 3);

        var useCase = new MarkInactiveSessionUseCase(activityPolicy);

        useCase.Execute(
            session,
            nowUtc: new DateTime(2026, 3, 12, 10, 0, 3, DateTimeKind.Utc));

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Inactive));
    }

    [Test]
    public void Should_not_mark_session_inactive_when_threshold_is_not_reached()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        session.RegisterPoll(new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var activityPolicy = new SessionActivityPolicy(
            expectedPollInterval: TimeSpan.FromSeconds(1),
            missedPollThreshold: 3);

        var useCase = new MarkInactiveSessionUseCase(activityPolicy);

        useCase.Execute(
            session,
            nowUtc: new DateTime(2026, 3, 12, 10, 0, 2, DateTimeKind.Utc));

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Queued));
    }

    [Test]
    public void Should_do_nothing_when_session_has_never_been_polled()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        var activityPolicy = new SessionActivityPolicy(
            expectedPollInterval: TimeSpan.FromSeconds(1),
            missedPollThreshold: 3);

        var useCase = new MarkInactiveSessionUseCase(activityPolicy);

        useCase.Execute(
            session,
            nowUtc: new DateTime(2026, 3, 12, 10, 0, 10, DateTimeKind.Utc));

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Queued));
    }
}