namespace SupportChat.Domain.Queues;

public enum QueueAdmissionResult
{
    MainQueue = 1,
    OverflowQueue = 2,
    Rejected = 3
}