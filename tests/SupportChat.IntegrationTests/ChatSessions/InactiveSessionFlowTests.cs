using Microsoft.Extensions.DependencyInjection;
using SupportChat.API.Contracts.Sessions;
using SupportChat.Application.Sessions;
using System.Net;
using System.Net.Http.Json;

namespace SupportChat.IntegrationTests.ChatSessions;

public class InactiveSessionFlowTests
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
    public async Task Should_mark_session_inactive_after_inactivity_processor_runs()
    {
        var createdAtUtc = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc);
        var polledAtUtc = createdAtUtc.AddSeconds(1);
        var processorRunAtUtc = polledAtUtc.AddSeconds(3);

        var createRequest = new CreateChatSessionHttpRequest
        {
            CurrentMainQueueCount = 5,
            CurrentOverflowQueueCount = 0,
            NowUtc = createdAtUtc
        };

        var createResponse = await _client.PostAsJsonAsync("/api/chat-sessions", createRequest);

        Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var created = await createResponse.Content.ReadFromJsonAsync<CreateChatSessionHttpResponse>();

        Assert.That(created, Is.Not.Null);
        Assert.That(created!.SessionId, Is.Not.Null);

        var sessionId = created.SessionId!.Value;

        var pollRequest = new RegisterPollHttpRequest
        {
            SessionCreatedAtUtc = createdAtUtc,
            PolledAtUtc = polledAtUtc
        };

        var pollResponse = await _client.PostAsJsonAsync($"/api/chat-sessions/{sessionId}/poll", pollRequest);

        Assert.That(pollResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        using (var scope = _factory.Services.CreateScope())
        {
            var processor = scope.ServiceProvider.GetRequiredService<InactiveSessionProcessor>();
            var processedCount = processor.Execute(processorRunAtUtc);

            Assert.That(processedCount, Is.EqualTo(1));
        }

        var getResponse = await _client.GetAsync($"/api/chat-sessions/{sessionId}");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var session = await getResponse.Content.ReadFromJsonAsync<GetChatSessionHttpResponse>();

        Assert.That(session, Is.Not.Null);
        Assert.That(session!.SessionId, Is.EqualTo(sessionId));
        Assert.That(session.Status, Is.EqualTo("Inactive"));
    }
}