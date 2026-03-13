namespace SupportChat.Application.Sessions;

public record CreateChatSessionRequest(
    Guid TeamId,
    DateTime CreatedAtUtc
);