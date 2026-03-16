using SupportChat.Application.Abstractions;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class GetChatSessionByIdUseCaseTests
{
    [Test]
    public void Should_return_session_when_it_exists()
    {
        var repository = new InMemoryTestChatSessionRepository();

        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc));

        repository.Add(session);

        var useCase = new GetChatSessionByIdUseCase(repository);

        var result = useCase.Execute(session.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(session.Id));
        Assert.That(result.Status, Is.EqualTo(SessionStatus.Queued));
    }

    [Test]
    public void Should_return_null_when_session_does_not_exist()
    {
        var repository = new InMemoryTestChatSessionRepository();
        var useCase = new GetChatSessionByIdUseCase(repository);

        var result = useCase.Execute(Guid.NewGuid());

        Assert.That(result, Is.Null);
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