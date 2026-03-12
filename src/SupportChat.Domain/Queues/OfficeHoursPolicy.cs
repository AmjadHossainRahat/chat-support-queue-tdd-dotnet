namespace SupportChat.Domain.Queues;

public class OfficeHoursPolicy
{
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }

    public OfficeHoursPolicy(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("End time must be later than start time.");
        }

        StartTime = startTime;
        EndTime = endTime;
    }

    public bool IsWithinOfficeHours(TimeOnly currentTime)
    {
        return currentTime >= StartTime && currentTime < EndTime;
    }
}