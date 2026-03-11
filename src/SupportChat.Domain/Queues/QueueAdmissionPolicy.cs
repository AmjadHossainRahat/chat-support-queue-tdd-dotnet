using SupportChat.Domain.Teams;

namespace SupportChat.Domain.Queues;

public class QueueAdmissionPolicy
{
    public QueueAdmissionResult Decide(
        Team mainTeam,
        int currentMainQueueCount,
        Team overflowTeam,
        int currentOverflowQueueCount,
        bool isWithinOfficeHours)
    {
        throw new NotImplementedException();
    }
}