namespace SupportChat.API.Contracts.Sessions;

public class RegisterPollHttpRequest
{
    public DateTime SessionCreatedAtUtc { get; set; }
    public DateTime PolledAtUtc { get; set; }
}