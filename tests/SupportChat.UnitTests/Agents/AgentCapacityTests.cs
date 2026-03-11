using SupportChat.Domain.Agents;

namespace SupportChat.UnitTests.Agents;

public class AgentCapacityTests
{
    [TestCase(Seniority.Junior, 4)]
    [TestCase(Seniority.Mid, 6)]
    [TestCase(Seniority.Senior, 8)]
    [TestCase(Seniority.TeamLead, 5)]
    public void Agent_capacity_should_match_expected_by_seniority(
        Seniority seniority,
        int expectedCapacity)
    {
        // Arrange
        var agent = new Agent(Guid.NewGuid(), seniority);

        // Act
        var capacity = agent.GetMaxConcurrentChats();

        // Assert
        Assert.That(capacity, Is.EqualTo(expectedCapacity));
    }
}
