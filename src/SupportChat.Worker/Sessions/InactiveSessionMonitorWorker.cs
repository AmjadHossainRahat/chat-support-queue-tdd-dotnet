using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupportChat.Application.Sessions;

namespace SupportChat.Worker.Sessions;

public class InactiveSessionMonitorWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InactiveSessionMonitorWorker> _logger;

    public InactiveSessionMonitorWorker(
        IServiceProvider serviceProvider,
        ILogger<InactiveSessionMonitorWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Inactive session monitor worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<InactiveSessionProcessor>();
            var repository = scope.ServiceProvider.GetRequiredService<SupportChat.Application.Abstractions.IChatSessionRepository>();

            _logger.LogDebug("Inactive session monitor cycle started");

            var queuedSessions = await repository.GetQueuedSessionsAsync(stoppingToken);
            var candidates = queuedSessions.ToList();

            var processedCount = await processor.ExecuteAsync(DateTime.UtcNow, stoppingToken);

            if (processedCount > 0)
            {
                foreach (var session in candidates)
                {
                    var refreshed = await repository.GetByIdAsync(session.Id, stoppingToken);

                    if (refreshed is not null && refreshed.Status == Domain.Sessions.SessionStatus.Inactive)
                    {
                        using (_logger.BeginScope(new Dictionary<string, object>
                        {
                            ["CorrelationId"] = refreshed.CorrelationId
                        }))
                        {
                            _logger.LogInformation(
                                "Marked chat session {SessionId} as inactive",
                                refreshed.Id);
                        }
                    }
                }
            }
            else
            {
                _logger.LogDebug("No inactive chat session was found in this cycle");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        _logger.LogInformation("Inactive session monitor worker stopped");
    }
}