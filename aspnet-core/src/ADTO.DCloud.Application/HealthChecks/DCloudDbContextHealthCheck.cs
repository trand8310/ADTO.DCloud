using System.Threading;
using System.Threading.Tasks;
using ADTO.DCloud.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
 
namespace ADTO.DCloud.HealthChecks;

/// <summary>
/// 数据库有效性检测,用于系统维护
/// </summary>
public class DCloudDbContextHealthCheck : IHealthCheck
{
    private readonly DatabaseCheckHelper _checkHelper;

    public DCloudDbContextHealthCheck(DatabaseCheckHelper checkHelper)
    {
        _checkHelper = checkHelper;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        if (_checkHelper.Exist("db"))
        {
            return Task.FromResult(HealthCheckResult.Healthy("数据库连接正常"));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("数据库无法连接"));
    }
}

