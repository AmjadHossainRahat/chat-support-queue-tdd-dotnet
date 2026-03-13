using NUnit.Framework;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class RegisterPollUseCaseTests
{
    private InMemoryTestChatSessionRepository _repository = null!;
    private RegisterPollUseCase _useCase = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new InMemoryTestChatSessionRepository();
        _useCase = new RegisterPollUseCase(_repository);
    }

    [Test]
    public void Should_update_last_poll_time()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        _repository.Add(session);

        var polledAtUtc = new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc);

        var result = _useCase.Execute(session.Id, polledAtUtc);

        Assert.That(result.LastPolledAtUtc, Is.EqualTo(polledAtUtc));
    }

    [Test]
    public void Should_not_change_session_status_when_registering_poll()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        _repository.Add(session);

        var result = _useCase.Execute(
            session.Id,
            new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc));

        Assert.That(result.Status, Is.EqualTo(SessionStatus.Queued));
    }

    [Test]
    public void Should_throw_when_poll_time_is_before_session_creation_time()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        _repository.Add(session);

        Assert.Throws<ArgumentException>(() =>
            _useCase.Execute(
                session.Id,
                new DateTime(2026, 3, 12, 9, 59, 59, DateTimeKind.Utc)));
    }

    [Test]
    public void Should_throw_when_session_does_not_exist()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _useCase.Execute(Guid.NewGuid(), new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc)));
    }

    private sealed class InMemoryTestChatSessionRepository : IChatSessionRepository
    {
        private readonly Dictionary<Guid, ChatSession> _sessions = new();

        public void Add(ChatSession session) => _sessions[session.Id] = session;

        public ChatSession? GetById(Guid sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public void Update(ChatSession session) => _sessions[session.Id] = session;
    }
}