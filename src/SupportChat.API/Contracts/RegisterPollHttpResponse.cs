namespace SupportChat.API.Contracts.Sessions;

public class RegisterPollHttpResponse
{
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? LastPolledAtUtc { get; set; }
}