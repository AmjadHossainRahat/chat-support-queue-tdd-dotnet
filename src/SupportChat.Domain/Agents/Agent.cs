namespace SupportChat.Domain.Agents;

public class Agent
{
    // TODO: make it configurable
    private const int BaseChatCapacity = 10;

    public Guid Id { get; }
    public Seniority Seniority { get; }
    public int ActiveChatCount { get; private set; }

    public Agent(Guid id, Seniority seniority, int activeChatCount = 0)
    {
        if (activeChatCount < 0) throw new ArgumentException("Active chat count cannot be negative.");

        Id = id;
        Seniority = seniority;
        ActiveChatCount = activeChatCount;
    }

    public int GetMaxConcurrentChats()
    {
        // TODO: make it configurable
        var multiplier = Seniority switch
        {
            Seniority.Junior => 0.4,
            Seniority.Mid => 0.6,
            Seniority.Senior => 0.8,
            Seniority.TeamLead => 0.5,
            _ => throw new ArgumentOutOfRangeException(nameof(Seniority), Seniority, null)
        };

        return (int)Math.Floor(BaseChatCapacity * multiplier);
    }

    public bool CanTakeMoreChats()
    {
        return ActiveChatCount < GetMaxConcurrentChats();
    }

    public void AssignChat()
    {
        if (!CanTakeMoreChats())
        {
            throw new InvalidOperationException("Agent is already at maximum chat capacity.");
        }

        ActiveChatCount++;
    }
}
