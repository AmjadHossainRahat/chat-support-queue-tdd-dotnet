using Microsoft.AspNetCore.Mvc;
using SupportChat.Api.Contracts.Sessions;
using SupportChat.API.Contracts.Sessions;
using SupportChat.API.Providers;
using SupportChat.Application.Sessions;
using SupportChat.Domain.Sessions;

namespace SupportChat.API.Controllers;

[ApiController]
[Route("api/chat-sessions")]
public class ChatSessionsController : ControllerBase
{
    private readonly CreateChatSessionUseCase _createChatSessionUseCase;

    public ChatSessionsController(CreateChatSessionUseCase createChatSessionUseCase)
    {
        _createChatSessionUseCase = createChatSessionUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateChatSessionHttpResponse), StatusCodes.Status200OK)]
    public ActionResult<CreateChatSessionHttpResponse> Create([FromBody] CreateChatSessionHttpRequest request)
    {
        var mainTeam = TeamProvider.CreateMainTeam();
        var overflowTeam = TeamProvider.CreateOverflowTeam();

        var result = _createChatSessionUseCase.Execute(
            mainTeam,
            request.CurrentMainQueueCount,
            overflowTeam,
            request.CurrentOverflowQueueCount,
            request.NowUtc);

        var response = new CreateChatSessionHttpResponse
        {
            AdmissionResult = result.AdmissionResult.ToString(),
            SessionId = result.SessionId
        };

        return Ok(response);
    }

    [HttpPost("{id:guid}/poll")]
    [ProducesResponseType(typeof(RegisterPollHttpResponse), StatusCodes.Status200OK)]
    public ActionResult<RegisterPollHttpResponse> RegisterPoll(Guid id, [FromBody] RegisterPollHttpRequest request)
    {
        var session = new ChatSession(id, request.SessionCreatedAtUtc);
        session.RegisterPoll(request.PolledAtUtc);

        var response = new RegisterPollHttpResponse
        {
            SessionId = session.Id,
            Status = session.Status.ToString(),
            LastPolledAtUtc = session.LastPolledAtUtc
        };

        return Ok(response);
    }
}