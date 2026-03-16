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
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<InactiveSessionProcessor>();

            var processedCount = processor.Execute(DateTime.UtcNow);

            if (processedCount > 0)
            {
                _logger.LogInformation(
                    "Marked {ProcessedCount} inactive chat session(s)",
                    processedCount);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}