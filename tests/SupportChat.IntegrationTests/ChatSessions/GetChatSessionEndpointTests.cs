using SupportChat.API.Contracts.Sessions;
using SupportChat.API.Constants;
using System.Net;
using System.Net.Http.Json;

namespace SupportChat.IntegrationTests.ChatSessions;

public class GetChatSessionEndpointTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
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
    public async Task Should_return_created_session_by_id()
    {
        var createRequest = new CreateChatSessionHttpRequest
        {
            CurrentMainQueueCount = 5,
            CurrentOverflowQueueCount = 0,
            NowUtc = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/chat-sessions", createRequest);
        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var created = await createResponse.Content.ReadFromJsonAsync<CreateChatSessionHttpResponse>();
        Assert.That(created, Is.Not.Null);
        Assert.That(created!.SessionId, Is.Not.Null);

        var response = await _client.GetAsync($"/api/chat-sessions/{created.SessionId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await response.Content.ReadFromJsonAsync<GetChatSessionHttpResponse>();

        Assert.That(body, Is.Not.Null);
        Assert.That(body!.SessionId, Is.EqualTo(created.SessionId));
        Assert.That(body.Status, Is.EqualTo("Queued"));
    }

    [Test]
    public async Task Should_return_not_found_when_session_does_not_exist()
    {
        var response = await _client.GetAsync($"/api/chat-sessions/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Should_generate_correlation_id_when_request_header_is_missing()
    {
        var response = await _client.GetAsync($"/api/chat-sessions/{Guid.NewGuid()}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(response.Headers.Contains(HttpHeaderNames.CorrelationId), Is.True);

        var correlationId = response.Headers.GetValues(HttpHeaderNames.CorrelationId).Single();

        Assert.That(Guid.TryParse(correlationId, out _), Is.True);
    }
}