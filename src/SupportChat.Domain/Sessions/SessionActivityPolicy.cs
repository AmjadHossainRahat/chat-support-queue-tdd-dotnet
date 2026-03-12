namespace SupportChat.Domain.Sessions;

public class SessionActivityPolicy
{
    public TimeSpan ExpectedPollInterval { get; }
    public int MissedPollThreshold { get; }

    public SessionActivityPolicy(TimeSpan expectedPollInterval, int missedPollThreshold)
    {
        if (expectedPollInterval <= TimeSpan.Zero)
        {
            throw new ArgumentException("Expected poll interval must be greater than zero.");
        }

        if (missedPollThreshold <= 0)
        {
            throw new ArgumentException("Missed poll threshold must be greater than zero.");
        }

        ExpectedPollInterval = expectedPollInterval;
        MissedPollThreshold = missedPollThreshold;
    }

    public bool IsInactive(DateTime lastPollAtUtc, DateTime nowUtc)
    {
        if (nowUtc < lastPollAtUtc)
        {
            throw new ArgumentException("Current time cannot be earlier than last poll time.");
        }

        var elapsed = nowUtc - lastPollAtUtc;
        var inactivityThreshold = TimeSpan.FromTicks(ExpectedPollInterval.Ticks * MissedPollThreshold);

        return elapsed >= inactivityThreshold;
    }
}