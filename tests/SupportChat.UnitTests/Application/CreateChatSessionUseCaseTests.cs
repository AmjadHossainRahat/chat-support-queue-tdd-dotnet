using SupportChat.Application.Abstractions;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Agents;
using SupportChat.Domain.Queues;
using SupportChat.Domain.Sessions;
using SupportChat.Domain.Teams;

namespace SupportChat.UnitTests.Application;

public class CreateChatSessionUseCaseTests
{
    private readonly OfficeHoursPolicy _officeHours = new(new TimeOnly(9, 0), new TimeOnly(17, 0));

    [Test]
    public void Should_create_session_in_main_queue_when_capacity_available()
    {
        var repository = new InMemoryTestChatSessionRepository();
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();

        var policy = new QueueAdmissionPolicy(_officeHours);
        var useCase = new CreateChatSessionUseCase(policy, repository);

        var result = useCase.Execute(
            mainTeam,
            currentMainQueueCount: 5,
            overflowTeam,
            currentOverflowQueueCount: 0,
            nowUtc: new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        Assert.That(result.AdmissionResult, Is.EqualTo(QueueAdmissionResult.MainQueue));
        Assert.That(result.SessionId, Is.Not.Null);
        Assert.That(repository.GetById(result.SessionId!.Value), Is.Not.Null);
    }

    [Test]
    public void Should_route_to_overflow_when_main_queue_full_during_office_hours()
    {
        var repository = new InMemoryTestChatSessionRepository();
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();

        var policy = new QueueAdmissionPolicy(_officeHours);
        var useCase = new CreateChatSessionUseCase(policy, repository);

        var result = useCase.Execute(
            mainTeam,
            currentMainQueueCount: mainTeam.GetQueueLimit(),
            overflowTeam,
            currentOverflowQueueCount: 1,
            nowUtc: new DateTime(2026, 3, 12, 11, 0, 0, DateTimeKind.Utc),
            "corr-1");

        Assert.That(result.AdmissionResult, Is.EqualTo(QueueAdmissionResult.OverflowQueue));
        Assert.That(result.SessionId, Is.Not.Null);
        Assert.That(repository.GetById(result.SessionId!.Value), Is.Not.Null);
    }

    [Test]
    public void Should_reject_when_main_queue_full_and_outside_office_hours()
    {
        var repository = new InMemoryTestChatSessionRepository();
        var mainTeam = CreateTeamA();
        var overflowTeam = CreateOverflowTeam();

        var policy = new QueueAdmissionPolicy(_officeHours);
        var useCase = new CreateChatSessionUseCase(policy, repository);

        var result = useCase.Execute(
            mainTeam,
            currentMainQueueCount: mainTeam.GetQueueLimit(),
            overflowTeam,
            currentOverflowQueueCount: 0,
            nowUtc: new DateTime(2026, 3, 12, 22, 0, 0, DateTimeKind.Utc),
            "corr-1");

        Assert.That(result.AdmissionResult, Is.EqualTo(QueueAdmissionResult.Rejected));
        Assert.That(result.SessionId, Is.Null);
    }

    private static Team CreateTeamA()
    {
        var agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.TeamLead),
            new(Guid.NewGuid(), Seniority.Mid),
            new(Guid.NewGuid(), Seniority.Mid),
            new(Guid.NewGuid(), Seniority.Junior)
        };

        return new Team(Guid.NewGuid(), "Team A", agents);
    }

    private static Team CreateOverflowTeam()
    {
        var agents = new List<Agent>
        {
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior),
            new(Guid.NewGuid(), Seniority.Junior)
        };

        return new Team(Guid.NewGuid(), "Overflow", agents);
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