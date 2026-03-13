using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class RegisterPollUseCaseTests
{
    [Test]
    public void Should_update_last_poll_time()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var useCase = new RegisterPollUseCase();

        var polledAtUtc = new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc);

        useCase.Execute(session, polledAtUtc);

        Assert.That(session.LastPolledAtUtc, Is.EqualTo(polledAtUtc));
    }

    [Test]
    public void Should_not_change_session_status_when_registering_poll()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var useCase = new RegisterPollUseCase();

        useCase.Execute(
            session,
            new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc));

        Assert.That(session.Status, Is.EqualTo(SessionStatus.Queued));
    }

    [Test]
    public void Should_throw_when_poll_time_is_before_session_creation_time()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var useCase = new RegisterPollUseCase();

        Assert.Throws<ArgumentException>(() =>
            useCase.Execute(
                session,
                new DateTime(2026, 3, 12, 9, 59, 59, DateTimeKind.Utc)));
    }
}