using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SupportChat.Application.Abstractions;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;
using SupportChat.Worker.Sessions;

namespace SupportChat.UnitTests.Worker;

public class InactiveSessionMonitorWorkerTests
{
    [Test]
    public async Task Should_mark_session_inactive_when_worker_runs()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(-2),
            "corr-1");

        session.RegisterPoll(DateTime.UtcNow.AddMinutes(-2));
        repository.Add(session);

        var processor = new InactiveSessionProcessor(
            repository,
            new MarkInactiveSessionUseCase(
                new SessionActivityPolicy(
                    expectedPollInterval: TimeSpan.FromSeconds(1),
                    missedPollThreshold: 3)));

        var serviceProvider = new ServiceCollection()
            .AddSingleton<SupportChat.Application.Abstractions.IChatSessionRepository>(repository)
            .AddSingleton(processor)
            .BuildServiceProvider();

        var worker = new InactiveSessionMonitorWorker(
            serviceProvider,
            NullLogger<InactiveSessionMonitorWorker>.Instance);

        await worker.StartAsync(CancellationToken.None);
        await Task.Delay(200);
        await worker.StopAsync(CancellationToken.None);

        var updated = repository.GetById(session.Id);

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Status, Is.EqualTo(SessionStatus.Inactive));
        Assert.That(updated.CorrelationId, Is.EqualTo("corr-1"));
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