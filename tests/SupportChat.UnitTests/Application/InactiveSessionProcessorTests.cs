using SupportChat.Application.Abstractions;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class InactiveSessionProcessorTests
{
    [Test]
    public void Should_mark_inactive_sessions_when_threshold_is_reached()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        session.RegisterPoll(new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));
        repository.Add(session);

        var activityPolicy = new SessionActivityPolicy(
            expectedPollInterval: TimeSpan.FromSeconds(1),
            missedPollThreshold: 3);

        var useCase = new MarkInactiveSessionUseCase(activityPolicy);
        var processor = new InactiveSessionProcessor(repository, useCase);

        var processedCount = processor.Execute(
            nowUtc: new DateTime(2026, 3, 12, 10, 0, 3, DateTimeKind.Utc));

        var updated = repository.GetById(session.Id);

        Assert.That(processedCount, Is.EqualTo(1));
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Status, Is.EqualTo(SessionStatus.Inactive));
    }

    [Test]
    public void Should_leave_active_sessions_unchanged_when_threshold_is_not_reached()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        session.RegisterPoll(new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));
        repository.Add(session);

        var activityPolicy = new SessionActivityPolicy(
            expectedPollInterval: TimeSpan.FromSeconds(1),
            missedPollThreshold: 3);

        var useCase = new MarkInactiveSessionUseCase(activityPolicy);
        var processor = new InactiveSessionProcessor(repository, useCase);

        var processedCount = processor.Execute(
            nowUtc: new DateTime(2026, 3, 12, 10, 0, 2, DateTimeKind.Utc));

        var updated = repository.GetById(session.Id);

        Assert.That(processedCount, Is.EqualTo(0));
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Status, Is.EqualTo(SessionStatus.Queued));
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

        public IReadOnlyCollection<ChatSession> GetQueuedSessions()
        {
            return _sessions.Values
                .Where(x => x.Status == SessionStatus.Queued)
                .OrderBy(x => x.CreatedAtUtc)
                .ToList();
        }

        public void Update(ChatSession session) => _sessions[session.Id] = session;
    }
}