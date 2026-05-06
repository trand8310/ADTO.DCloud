using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ADTO.DCloud.Desktop.Services;

public sealed class DesktopHeartbeatService(IConfiguration configuration, ISessionService sessionService, ILogger<DesktopHeartbeatService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var seconds = configuration.GetValue("DesktopShell:HeartbeatSeconds", 15);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(seconds, 5)));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            logger.LogDebug("DCloud desktop heartbeat. Authenticated={IsAuthenticated}", sessionService.IsAuthenticated);
        }
    }
}
