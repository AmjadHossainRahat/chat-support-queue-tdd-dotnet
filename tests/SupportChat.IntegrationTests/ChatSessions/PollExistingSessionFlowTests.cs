using Microsoft.AspNetCore.Mvc.Testing;
using SupportChat.API.Contracts.Sessions;
using System.Net;
using System.Net.Http.Json;

namespace SupportChat.IntegrationTests.ChatSessions;

public class PollExistingSessionFlowTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task Should_create_session_then_poll_same_session()
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

        var sessionId = createdBody.SessionId!.Value;

        var pollRequest = new RegisterPollHttpRequest
        {
            SessionCreatedAtUtc = createRequest.NowUtc,
            PolledAtUtc = createRequest.NowUtc.AddSeconds(1)
        };

        var pollResponse = await _client.PostAsJsonAsync($"/api/chat-sessions/{sessionId}/poll", pollRequest);

        Assert.That(pollResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var pollBody = await pollResponse.Content.ReadFromJsonAsync<RegisterPollHttpResponse>();

        Assert.That(pollBody, Is.Not.Null);
        Assert.That(pollBody!.SessionId, Is.EqualTo(sessionId));
        Assert.That(pollBody.LastPolledAtUtc, Is.EqualTo(pollRequest.PolledAtUtc));
    }
}