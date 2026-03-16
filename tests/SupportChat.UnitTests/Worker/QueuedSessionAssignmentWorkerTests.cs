using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Assignments;
using SupportChat.Domain.Agents;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Sessions;
using SupportChat.Worker.Assignment;

namespace SupportChat.UnitTests.Worker;

public class QueuedSessionAssignmentWorkerTests
{
    [Test]
    public async Task Should_assign_waiting_session_when_worker_runs()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        repository.Add(session);

        var availableAgent = new Agent(
            Guid.NewGuid(),
            Seniority.Junior,
            activeChatCount: 0);

        var processor = new QueuedSessionAssignmentProcessor(
            new InMemoryTestAgentProvider(new[] { availableAgent }),
            new AssignNextQueuedSessionUseCase(
                repository,
                new AssignWaitingSessionUseCase(new AssignmentPolicy())));

        var serviceProvider = new ServiceCollection()
            .AddSingleton(processor)
            .BuildServiceProvider();

        var worker = new QueuedSessionAssignmentWorker(
            serviceProvider,
            NullLogger<QueuedSessionAssignmentWorker>.Instance);

        await worker.StartAsync(CancellationToken.None);
        await Task.Delay(200);
        await worker.StopAsync(CancellationToken.None);

        var updated = repository.GetById(session.Id);

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Status, Is.EqualTo(SessionStatus.Assigned));
        Assert.That(updated.AssignedAgentId, Is.EqualTo(availableAgent.Id));
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