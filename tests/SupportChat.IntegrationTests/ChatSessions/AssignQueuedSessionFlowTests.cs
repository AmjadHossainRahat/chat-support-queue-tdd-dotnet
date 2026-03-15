using Microsoft.Extensions.DependencyInjection;
using SupportChat.API.Contracts.Sessions;
using SupportChat.Application.Assignments;
using System.Net;
using System.Net.Http.Json;

namespace SupportChat.IntegrationTests.ChatSessions;

/// <summary>
/// verify the end-to-end assignment flow:
/// create a queued session through the API
/// execute the assignment processor
/// read the session again through the API
/// confirm it is now Assigned
/// </summary>
public class AssignQueuedSessionFlowTests
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
    public async Task Should_assign_created_queued_session_after_processor_runs()
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

        using (var scope = _factory.Services.CreateScope())
        {
            var processor = scope.ServiceProvider.GetRequiredService<QueuedSessionAssignmentProcessor>();
            var assignedSession = processor.Execute();

            Assert.That(assignedSession, Is.Not.Null);
            Assert.That(assignedSession!.Id, Is.EqualTo(created.SessionId!.Value));
        }

        var getResponse = await _client.GetAsync($"/api/chat-sessions/{created.SessionId}");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var session = await getResponse.Content.ReadFromJsonAsync<GetChatSessionHttpResponse>();

        Assert.That(session, Is.Not.Null);
        Assert.That(session!.SessionId, Is.EqualTo(created.SessionId));
        Assert.That(session.Status, Is.EqualTo("Assigned"));
        Assert.That(session.AssignedAgentId, Is.Not.Null);
    }
}