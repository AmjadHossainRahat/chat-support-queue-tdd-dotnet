using Microsoft.AspNetCore.Mvc;
using SupportChat.API.Contracts.Sessions;
using SupportChat.API.Providers;
using SupportChat.Application.Sessions;

namespace SupportChat.Api.Controllers;

[ApiController]
[Route("api/chat-sessions")]
public class ChatSessionsController : ControllerBase
{
    private readonly CreateChatSessionUseCase _createChatSessionUseCase;
    private readonly RegisterPollUseCase _registerPollUseCase;

    public ChatSessionsController(
        CreateChatSessionUseCase createChatSessionUseCase,
        RegisterPollUseCase registerPollUseCase)
    {
        _createChatSessionUseCase = createChatSessionUseCase;
        _registerPollUseCase = registerPollUseCase;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<RegisterPollHttpResponse> RegisterPoll(Guid id, [FromBody] RegisterPollHttpRequest request)
    {
        try
        {
            var session = _registerPollUseCase.Execute(id, request.PolledAtUtc);

            var response = new RegisterPollHttpResponse
            {
                SessionId = session.Id,
                Status = session.Status.ToString(),
                LastPolledAtUtc = session.LastPolledAtUtc
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("was not found"))
        {
            return NotFound();
        }
    }
}