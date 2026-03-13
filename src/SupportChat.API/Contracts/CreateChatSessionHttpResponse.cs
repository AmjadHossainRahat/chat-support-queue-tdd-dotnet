namespace SupportChat.API.Contracts.Sessions;

public class CreateChatSessionHttpResponse
{
    public string AdmissionResult { get; set; } = string.Empty;
    public Guid? SessionId { get; set; }
}