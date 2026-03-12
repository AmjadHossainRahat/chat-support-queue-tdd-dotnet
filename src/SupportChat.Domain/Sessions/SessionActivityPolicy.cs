namespace SupportChat.Domain.Sessions;

public class SessionActivityPolicy
{
    public TimeSpan ExpectedPollInterval { get; }
    public int MissedPollThreshold { get; }

    public SessionActivityPolicy(TimeSpan expectedPollInterval, int missedPollThreshold)
    {

        ExpectedPollInterval = expectedPollInterval;
        MissedPollThreshold = missedPollThreshold;
    }

    public bool IsInactive(DateTime lastPollAtUtc, DateTime nowUtc)
    {
        throw new NotImplementedException();
    }
}