using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Domain.Agents;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class QueuedSessionAssignmentProcessorTests
{
    [Test]
    public void Should_assign_next_queued_session_using_available_agents()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        repository.Add(session);

        var junior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 0);
        var provider = new InMemoryTestAgentProvider(new[] { junior });

        var processor = new QueuedSessionAssignmentProcessor(
            provider,
            new AssignNextQueuedSessionUseCase(
                repository,
                new AssignWaitingSessionUseCase(new Domain.Assignments.AssignmentPolicy())));

        var result = processor.Execute();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(session.Id));
        Assert.That(result.Status, Is.EqualTo(SessionStatus.Assigned));
        Assert.That(result.AssignedAgentId, Is.EqualTo(junior.Id));
    }

    [Test]
    public void Should_return_null_when_no_session_is_waiting()
    {
        var repository = new InMemoryTestChatSessionRepository();
        var provider = new InMemoryTestAgentProvider(new[]
        {
            new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 0)
        });

        var processor = new QueuedSessionAssignmentProcessor(
            provider,
            new AssignNextQueuedSessionUseCase(
                repository,
                new AssignWaitingSessionUseCase(new Domain.Assignments.AssignmentPolicy())));

        var result = processor.Execute();

        Assert.That(result, Is.Null);
    }

    private sealed class InMemoryTestAgentProvider : IAgentProvider
    {
        private readonly IReadOnlyCollection<Agent> _agents;

        public InMemoryTestAgentProvider(IReadOnlyCollection<Agent> agents)
        {
            _agents = agents;
        }

        public IReadOnlyCollection<Agent> GetAvailableAgents() => _agents;
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