using NUnit.Framework;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Sessions;

public class ChatSessionTests
{
    [Test]
    public void New_session_should_start_in_queued_status()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Queued));
    }

    [Test]
    public void Register_poll_should_update_last_polled_time()
    {
        var createdAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);
        var polledAt = createdAt.AddSeconds(1);

        var session = new ChatSession(Guid.NewGuid(), createdAt);

        session.RegisterPoll(polledAt);

        Assert.That(session.LastPolledAtUtc, Is.EqualTo(polledAt));
    }

    [Test]
    public void Register_poll_should_throw_when_poll_time_is_before_creation_time()
    {
        var createdAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);
        var invalidPollTime = createdAt.AddSeconds(-1);

        var session = new ChatSession(Guid.NewGuid(), createdAt);

        Assert.Throws<ArgumentException>(() => session.RegisterPoll(invalidPollTime));
    }

    [Test]
    public void Mark_assigned_should_set_status_to_assigned_and_store_agent_id()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var agentId = Guid.NewGuid();

        session.MarkAssigned(agentId);

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Assigned));
        Assert.That(session.AssignedAgentId, Is.EqualTo(agentId));
    }

    [Test]
    public void Mark_inactive_should_set_status_to_inactive()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        session.MarkInactive();

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Inactive));
    }
}