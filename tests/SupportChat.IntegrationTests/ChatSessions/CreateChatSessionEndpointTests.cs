using Microsoft.AspNetCore.Mvc.Testing;
using SupportChat.API.Contracts.Sessions;
using System.Net;
using System.Net.Http.Json;

namespace SupportChat.IntegrationTests.ChatSessions;

public class CreateChatSessionEndpointTests
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
    public async Task Should_create_chat_session_in_main_queue_when_capacity_is_available()
    {
        var request = new CreateChatSessionHttpRequest
        {
            CurrentMainQueueCount = 5,
            CurrentOverflowQueueCount = 0,
            NowUtc = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc)
        };

        var response = await _client.PostAsJsonAsync("/api/chat-sessions", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var body = await response.Content.ReadFromJsonAsync<CreateChatSessionHttpResponse>();

        Assert.That(body, Is.Not.Null);
        Assert.That(body!.AdmissionResult, Is.EqualTo("MainQueue"));
        Assert.That(body.SessionId, Is.Not.Null);
    }
}