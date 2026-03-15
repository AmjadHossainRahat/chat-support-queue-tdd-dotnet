using System.Net;
using System.Net.Http.Json;
using SupportChat.API.Contracts.Sessions;

namespace SupportChat.IntegrationTests.ChatSessions;

public class RegisterPollEndpointTests
{
    // private WebApplicationFactory<Program> _factory = null!;
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        // _factory = new WebApplicationFactory<Program>();
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task Should_register_poll_for_existing_session_and_return_ok()
    {
        var createRequest = new CreateChatSessionHttpRequest
        {
            CurrentMainQueueCount = 5,
            CurrentOverflowQueueCount = 0,
            NowUtc = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/chat-sessions", createRequest);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var createdBody = await createResponse.Content.ReadFromJsonAsync<CreateChatSessionHttpResponse>();
        Assert.That(createdBody, Is.Not.Null);
        Assert.That(createdBody!.SessionId, Is.Not.Null);

        var sessionId = createdBody.SessionId.Value;

        var pollRequest = new RegisterPollHttpRequest
        {
            SessionCreatedAtUtc = createRequest.NowUtc,
            PolledAtUtc = createRequest.NowUtc.AddSeconds(1)
        };

        var response = await _client.PostAsJsonAsync($"/api/chat-sessions/{sessionId}/poll", pollRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await response.Content.ReadFromJsonAsync<RegisterPollHttpResponse>();

        Assert.That(body, Is.Not.Null);
        Assert.That(body!.SessionId, Is.EqualTo(sessionId));
        Assert.That(body.Status, Is.EqualTo("Queued"));
        Assert.That(body.LastPolledAtUtc, Is.EqualTo(pollRequest.PolledAtUtc));
    }

    [Test]
    public async Task Should_return_not_found_when_polling_unknown_session()
    {
        var request = new RegisterPollHttpRequest
        {
            SessionCreatedAtUtc = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            PolledAtUtc = new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc)
        };

        var response = await _client.PostAsJsonAsync($"/api/chat-sessions/{Guid.NewGuid()}/poll", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}