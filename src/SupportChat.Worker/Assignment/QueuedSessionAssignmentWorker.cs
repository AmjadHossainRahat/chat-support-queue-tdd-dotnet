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
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<QueuedSessionAssignmentProcessor>();

            var assignedSession = processor.Execute();

            if (assignedSession is not null)
            {
                _logger.LogInformation(
                    "Assigned queued session {SessionId} to agent {AgentId}",
                    assignedSession.Id,
                    assignedSession.AssignedAgentId);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}