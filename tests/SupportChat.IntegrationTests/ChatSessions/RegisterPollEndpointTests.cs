using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using SupportChat.Api.Contracts.Sessions;
using SupportChat.API.Contracts.Sessions;

namespace SupportChat.IntegrationTests.ChatSessions;

public class RegisterPollEndpointTests
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
    public async Task Should_register_poll_and_return_ok()
    {
        var sessionId = Guid.NewGuid();

        var request = new RegisterPollHttpRequest
        {
            SessionCreatedAtUtc = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
            PolledAtUtc = new DateTime(2026, 3, 12, 10, 0, 1, DateTimeKind.Utc)
        };

        var response = await _client.PostAsJsonAsync($"/api/chat-sessions/{sessionId}/poll", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await response.Content.ReadFromJsonAsync<RegisterPollHttpResponse>();

        Assert.That(body, Is.Not.Null);
        Assert.That(body!.SessionId, Is.EqualTo(sessionId));
        Assert.That(body.Status, Is.EqualTo("Queued"));
        Assert.That(body.LastPolledAtUtc, Is.EqualTo(request.PolledAtUtc));
    }
}