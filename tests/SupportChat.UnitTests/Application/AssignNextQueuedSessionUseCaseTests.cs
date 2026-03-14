using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Domain.Agents;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class AssignNextQueuedSessionUseCaseTests
{
    [Test]
    public void Should_assign_oldest_queued_session_to_best_available_agent()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var olderSession = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        var newerSession = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 5, DateTimeKind.Utc));

        repository.Add(olderSession);
        repository.Add(newerSession);

        var junior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 0);
        var mid = new Agent(Guid.NewGuid(), Seniority.Mid, activeChatCount: 0);

        var useCase = new AssignNextQueuedSessionUseCase(
            repository,
            new AssignWaitingSessionUseCase(new AssignmentPolicy()));

        var assignedSession = useCase.Execute(new[] { mid, junior });

        Assert.That(assignedSession, Is.Not.Null);
        Assert.That(assignedSession!.Id, Is.EqualTo(olderSession.Id));
        Assert.That(assignedSession.Status, Is.EqualTo(SessionStatus.Assigned));
        Assert.That(assignedSession.AssignedAgentId, Is.EqualTo(junior.Id));
        Assert.That(junior.ActiveChatCount, Is.EqualTo(1));
    }

    [Test]
    public void Should_return_null_when_there_is_no_queued_session()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var useCase = new AssignNextQueuedSessionUseCase(
            repository,
            new AssignWaitingSessionUseCase(new AssignmentPolicy()));

        var result = useCase.Execute(new[]
        {
            new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 0)
        });

        Assert.That(result, Is.Null);
    }

    [Test]
    public void Should_throw_when_no_eligible_agent_exists_for_queued_session()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        repository.Add(session);

        var junior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 4);
        var mid = new Agent(Guid.NewGuid(), Seniority.Mid, activeChatCount: 6);

        var useCase = new AssignNextQueuedSessionUseCase(
            repository,
            new AssignWaitingSessionUseCase(new AssignmentPolicy()));

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(new[] { junior, mid }));
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