using SupportChat.Domain.Teams;

namespace SupportChat.Domain.Queues;

public class QueueAdmissionPolicy
{
    private readonly OfficeHoursPolicy _officeHoursPolicy;

    public QueueAdmissionPolicy(OfficeHoursPolicy officeHoursPolicy)
    {
        _officeHoursPolicy = officeHoursPolicy;
    }

    public QueueAdmissionResult Decide(
        Team mainTeam,
        int currentMainQueueCount,
        Team overflowTeam,
        int currentOverflowQueueCount,
        TimeOnly currentTime)
    {
        var mainQueueLimit = mainTeam.GetQueueLimit();

        if (currentMainQueueCount < mainQueueLimit)
        {
            return QueueAdmissionResult.MainQueue;
        }

        if (!_officeHoursPolicy.IsWithinOfficeHours(currentTime))
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