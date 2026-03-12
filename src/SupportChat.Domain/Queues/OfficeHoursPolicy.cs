namespace SupportChat.Domain.Queues;

public class OfficeHoursPolicy
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    public OfficeHoursPolicy(TimeOnly startTime, TimeOnly endTime)
    {
        throw new NotImplementedException();
    }

    public bool IsWithinOfficeHours(TimeOnly currentTime)
    {
        throw new NotImplementedException();
    }
}