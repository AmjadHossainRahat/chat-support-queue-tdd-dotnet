using NUnit.Framework;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Sessions;

public class SessionActivityPolicyTests
{
    private readonly SessionActivityPolicy _policy = new(
        expectedPollInterval: TimeSpan.FromSeconds(1),
        missedPollThreshold: 3);

    [Test]
    public void Should_keep_session_active_when_missed_polls_are_below_threshold()
    {
        var lastPollAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);
        var now = lastPollAt.AddSeconds(2);

        var result = _policy.IsInactive(lastPollAt, now);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Should_mark_session_inactive_when_missed_polls_reach_threshold()
    {
        var lastPollAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);
        var now = lastPollAt.AddSeconds(3);

        var result = _policy.IsInactive(lastPollAt, now);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_mark_session_inactive_when_missed_polls_exceed_threshold()
    {
        var lastPollAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);
        var now = lastPollAt.AddSeconds(5);

        var result = _policy.IsInactive(lastPollAt, now);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Should_throw_when_now_is_earlier_than_last_poll_time()
    {
        var lastPollAt = new DateTime(2026, 3, 12, 10, 0, 5, DateTimeKind.Utc);
        var now = lastPollAt.AddSeconds(-1);

        Assert.Throws<ArgumentException>(() => _policy.IsInactive(lastPollAt, now));
    }
}