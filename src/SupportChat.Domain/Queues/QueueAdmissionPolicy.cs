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
        var mainQueueLimit = mainTeam.GetQueueLimit();

        // Main queue has capacity
        if (currentMainQueueCount < mainQueueLimit)
        {
            return QueueAdmissionResult.MainQueue;
        }

        // Main queue full
        if (!isWithinOfficeHours)
        {
            return QueueAdmissionResult.Rejected;
        }

        var overflowQueueLimit = overflowTeam.GetQueueLimit();

        if (currentOverflowQueueCount < overflowQueueLimit)
        {
            return QueueAdmissionResult.OverflowQueue;
        }

        return QueueAdmissionResult.Rejected;
    }
}