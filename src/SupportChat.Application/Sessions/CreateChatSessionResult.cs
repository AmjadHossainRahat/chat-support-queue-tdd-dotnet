using SupportChat.Domain.Queues;

namespace SupportChat.Application.Sessions;

public record CreateChatSessionResult(
    QueueAdmissionResult AdmissionResult,
    Guid? SessionId
);