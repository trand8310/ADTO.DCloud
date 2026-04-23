using System;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp.Runtime.Caching;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ADTO.DCloud.HealthChecks;
/// <summary>
/// 缓存管理有效性检测,用于系统维护
/// </summary>
public class CacheHealthCheck : IHealthCheck
{
    private readonly ICacheManager _cacheManager;

    public CacheHealthCheck(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    // 尝试从缓存中设置和获取数据。
    // 如果redis缓存被启用，它将尝试连接到redis来设置和获取缓存数据。如果它不会抛出异常，就意味着redis运行良好。
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var cacheManager = _cacheManager.GetCache("TestCache");

            var testKey = "Test-" + Guid.NewGuid();

            await cacheManager.SetAsync(testKey, "123");
            
            await cacheManager.GetOrDefaultAsync(testKey);

            return HealthCheckResult.Healthy("缓存检查正常。（如果你正在使用Redis， Redis也被检查）");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("缓存检查不正常。（如果你正在使用Redis， Redis也被检查）" + e.Message);
        }
    }
}
