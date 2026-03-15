namespace SupportChat.API.Contracts.Sessions;

public class GetChatSessionHttpResponse
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastPolledAtUtc { get; set; }
    public Guid? AssignedAgentId { get; set; }
}