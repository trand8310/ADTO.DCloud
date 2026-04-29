using ADTOSharp.Runtime.Caching.Redis;
using ADTOSharp.Runtime.Caching.Redis.RealTime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ADTO.DCloud.Web.Host.BackgroundJobs;

/// <summary>
/// SignalR 在线连接心跳续期 + 僵尸连接清理。
/// </summary>
public class SignalROnlineClientJanitorHostedService : BackgroundService
{
    private readonly ILogger<SignalROnlineClientJanitorHostedService> _logger;
    private readonly RedisOnlineClientStore _store;
    private readonly ADTOSharpRedisCacheOptions _options;
    private DateTime _nextCleanupAtUtc;

    public SignalROnlineClientJanitorHostedService(
        ILogger<SignalROnlineClientJanitorHostedService> logger,
        RedisOnlineClientStore store,
        ADTOSharpRedisCacheOptions options)
    {
        _logger = logger;
        _store = store;
        _options = options;
        _nextCleanupAtUtc = DateTime.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SignalR janitor started. InstanceId={InstanceId}", _store.InstanceId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var refreshed = await _store.RefreshInstanceHeartbeatsAsync(stoppingToken);
                _logger.LogDebug("SignalR heartbeat refreshed count={Count}, instance={InstanceId}", refreshed, _store.InstanceId);

                if (DateTime.UtcNow >= _nextCleanupAtUtc)
                {
                    var removed = await _store.CleanupStaleClientsAsync(_options.OnlineClientCleanupBatchSize, stoppingToken);
                    _logger.LogInformation("SignalR stale clients cleanup removed={Count}, instance={InstanceId}", removed, _store.InstanceId);
                    _nextCleanupAtUtc = DateTime.UtcNow.AddSeconds(Math.Max(5, _options.OnlineClientCleanupIntervalSeconds));
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR janitor loop failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(5, _options.OnlineClientHeartbeatRefreshIntervalSeconds)), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            var removed = await _store.CleanupCurrentInstanceAsync(cancellationToken);
            _logger.LogInformation("SignalR instance cleanup completed. removed={Count}, instance={InstanceId}", removed, _store.InstanceId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR instance cleanup failed during stopping.");
        }

        await base.StopAsync(cancellationToken);
    }
}