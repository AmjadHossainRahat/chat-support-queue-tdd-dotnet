namespace SupportChat.API.Contracts.Sessions;

public class CreateChatSessionHttpRequest
{
    public int CurrentMainQueueCount { get; set; }
    public int CurrentOverflowQueueCount { get; set; }
    public DateTime NowUtc { get; set; }
}