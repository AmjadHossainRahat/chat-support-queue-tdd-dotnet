using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportChat.Application.Assignments;

namespace SupportChat.Worker.Assignment;

public class QueuedSessionAssignmentWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueuedSessionAssignmentWorker> _logger;

    public QueuedSessionAssignmentWorker(
        IServiceProvider serviceProvider,
        ILogger<QueuedSessionAssignmentWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued session assignment worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<QueuedSessionAssignmentProcessor>();

            _logger.LogDebug("Queued session assignment cycle started");

            var assignedSession = await processor.ExecuteAsync(stoppingToken);

            if (assignedSession is not null)
            {
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["CorrelationId"] = assignedSession.CorrelationId
                }))
                {
                    _logger.LogInformation(
                        "Assigned queued session {SessionId} to agent {AgentId}",
                        assignedSession.Id,
                        assignedSession.AssignedAgentId);
                }
            }
            else
            {
                _logger.LogDebug("No queued session was assigned in this cycle");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        _logger.LogInformation("Queued session assignment worker stopped");
    }
}