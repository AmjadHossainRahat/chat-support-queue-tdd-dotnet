using NUnit.Framework;
using SupportChat.Application.Assignments;
using SupportChat.Domain.Agents;
using SupportChat.Domain.Assignments;
using SupportChat.Domain.Sessions;

namespace SupportChat.UnitTests.Application;

public class AssignWaitingSessionUseCaseTests
{
    [Test]
    public void Should_assign_session_to_selected_agent_and_mark_session_as_assigned()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        var junior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 0);
        var mid = new Agent(Guid.NewGuid(), Seniority.Mid, activeChatCount: 0);

        var useCase = new AssignWaitingSessionUseCase(new AssignmentPolicy());

        var assignedAgent = useCase.Execute(session, new[] { mid, junior });

        Assert.That(assignedAgent, Is.EqualTo(junior));
        Assert.That(junior.ActiveChatCount, Is.EqualTo(1));
        Assert.That(session.Status, Is.EqualTo(SessionStatus.Assigned));
        Assert.That(session.AssignedAgentId, Is.EqualTo(junior.Id));
    }

    [Test]
    public void Should_choose_less_busy_agent_within_same_seniority()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        var junior1 = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 2);
        var junior2 = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 1);

        var useCase = new AssignWaitingSessionUseCase(new AssignmentPolicy());

        var assignedAgent = useCase.Execute(session, new[] { junior1, junior2 });

        Assert.That(assignedAgent, Is.EqualTo(junior2));
        Assert.That(junior2.ActiveChatCount, Is.EqualTo(2));
        Assert.That(session.Status, Is.EqualTo(SessionStatus.Assigned));
        Assert.That(session.AssignedAgentId, Is.EqualTo(junior2.Id));
    }

    [Test]
    public void Should_throw_when_no_eligible_agent_is_available()
    {
        var session = new ChatSession(
            Guid.NewGuid(),
            new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            "corr-1");

        var junior = new Agent(Guid.NewGuid(), Seniority.Junior, activeChatCount: 4);
        var mid = new Agent(Guid.NewGuid(), Seniority.Mid, activeChatCount: 6);

        var useCase = new AssignWaitingSessionUseCase(new AssignmentPolicy());

        Assert.Throws<InvalidOperationException>(() =>
            useCase.Execute(session, new[] { junior, mid }));
    }
}