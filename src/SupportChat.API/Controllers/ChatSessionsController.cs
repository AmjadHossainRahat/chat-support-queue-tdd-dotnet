using Microsoft.AspNetCore.Mvc;
using SupportChat.API.Contracts.Sessions;
using SupportChat.API.Middleware;
using SupportChat.API.Providers;
using SupportChat.Application.Sessions;

namespace SupportChat.Api.Controllers;

[ApiController]
[Route("api/chat-sessions")]
public class ChatSessionsController : ControllerBase
{
    private readonly CreateChatSessionUseCase _createChatSessionUseCase;
    private readonly RegisterPollUseCase _registerPollUseCase;
    private readonly GetChatSessionByIdUseCase _getChatSessionByIdUseCase;
    private readonly ILogger<ChatSessionsController> _logger;

    public ChatSessionsController(
        CreateChatSessionUseCase createChatSessionUseCase,
        RegisterPollUseCase registerPollUseCase,
        GetChatSessionByIdUseCase getChatSessionByIdUseCase,
        ILogger<ChatSessionsController> logger)
    {
        _createChatSessionUseCase = createChatSessionUseCase;
        _registerPollUseCase = registerPollUseCase;
        _getChatSessionByIdUseCase = getChatSessionByIdUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new chat session.
    /// </summary>
    /// <remarks>
    /// Applies queue admission rules based on current queue state and submission time.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(CreateChatSessionHttpResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CreateChatSessionHttpResponse>> Create(
        [FromBody] CreateChatSessionHttpRequest request,
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items[CorrelationIdMiddleware.HttpContextItemKey]?.ToString()
                            ?? HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Creating chat session with main queue count {MainQueueCount} and overflow queue count {OverflowQueueCount} at {NowUtc}",
            request.CurrentMainQueueCount,
            request.CurrentOverflowQueueCount,
            request.NowUtc);

        var mainTeam = TeamProvider.CreateMainTeam();
        var overflowTeam = TeamProvider.CreateOverflowTeam();

        var result = await _createChatSessionUseCase.ExecuteAsync(
            mainTeam,
            request.CurrentMainQueueCount,
            overflowTeam,
            request.CurrentOverflowQueueCount,
            request.NowUtc,
            correlationId,
            cancellationToken);

        _logger.LogInformation(
            "Create chat session completed with admission result {AdmissionResult} and session id {SessionId}",
            result.AdmissionResult,
            result.SessionId);

        var response = new CreateChatSessionHttpResponse
        {
            AdmissionResult = result.AdmissionResult.ToString(),
            SessionId = result.SessionId
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets a chat session by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetChatSessionHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetChatSessionHttpResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chat session by id {SessionId}", id);

        var session = await _getChatSessionByIdUseCase.ExecuteAsync(id, cancellationToken);

        if (session is null)
        {
            _logger.LogWarning("Chat session {SessionId} was not found", id);
            return NotFound();
        }

        _logger.LogInformation(
            "Chat session {SessionId} found with status {Status}",
            session.Id,
            session.Status);

        var response = new GetChatSessionHttpResponse
        {
            SessionId = session.Id,
            Status = session.Status.ToString(),
            CreatedAtUtc = session.CreatedAtUtc,
            LastPolledAtUtc = session.LastPolledAtUtc,
            AssignedAgentId = session.AssignedAgentId
        };

        return Ok(response);
    }

    /// <summary>
    /// Registers a polling event for a chat session.
    /// </summary>
    [HttpPost("{id:guid}/poll")]
    [ProducesResponseType(typeof(RegisterPollHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegisterPollHttpResponse>> RegisterPoll(
        Guid id,
        [FromBody] RegisterPollHttpRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Registering poll for chat session {SessionId} at {PolledAtUtc}",
            id,
            request.PolledAtUtc);

        var session = await _registerPollUseCase.ExecuteAsync(
            id,
            request.PolledAtUtc,
            cancellationToken);

        _logger.LogInformation(
            "Poll registered for chat session {SessionId}; current status is {Status}",
            session.Id,
            session.Status);

        var response = new RegisterPollHttpResponse
        {
            SessionId = session.Id,
            Status = session.Status.ToString(),
            LastPolledAtUtc = session.LastPolledAtUtc
        };

        return Ok(response);
    }
}