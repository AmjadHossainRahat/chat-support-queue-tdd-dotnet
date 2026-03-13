using NUnit.Framework;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Sessions;

public class ChatSessionLifecycleTests
{
    [Test]
    public void Should_not_allow_assigning_an_inactive_session()
    {
        var session = CreateSession();

        session.MarkInactive();

        Assert.Throws<InvalidOperationException>(() => session.MarkAssigned(Guid.NewGuid()));
    }

    [Test]
    public void Should_not_allow_assigning_a_completed_session()
    {
        var session = CreateAssignedSession();

        session.MarkCompleted();

        Assert.Throws<InvalidOperationException>(() => session.MarkAssigned(Guid.NewGuid()));
    }

    [Test]
    public void Should_not_allow_assigning_a_rejected_session()
    {
        var session = CreateSession();

        session.MarkRejected();

        Assert.Throws<InvalidOperationException>(() => session.MarkAssigned(Guid.NewGuid()));
    }

    [Test]
    public void Should_not_allow_completing_a_session_that_is_not_assigned()
    {
        var session = CreateSession();

        Assert.Throws<InvalidOperationException>(() => session.MarkCompleted());
    }

    [Test]
    public void Should_allow_completing_an_assigned_session()
    {
        var session = CreateAssignedSession();

        session.MarkCompleted();

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Completed));
    }

    private static ChatSession CreateSession()
    {
        return new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));
    }

    private static ChatSession CreateAssignedSession()
    {
        var session = CreateSession();
        session.MarkAssigned(Guid.NewGuid());
        return session;
    }
}