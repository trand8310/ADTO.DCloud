using System;
using System.Threading;
using System.Threading.Tasks;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ADTO.DCloud.EntityFrameworkCore;

namespace ADTO.DCloud.HealthChecks;
/// <summary>
/// 用户数据有效性检测,用于系统维护
/// </summary>
public class DCloudDbContextUsersHealthCheck : IHealthCheck
{
    private readonly IDbContextProvider<DCloudDbContext> _dbContextProvider;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public DCloudDbContextUsersHealthCheck(
        IDbContextProvider<DCloudDbContext> dbContextProvider,
        IUnitOfWorkManager unitOfWorkManager
        )
    {
        _dbContextProvider = dbContextProvider;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                // 对于单租户模式，必须切换到主机(租户ID 为空的情况就是主机模式)。
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var dbContext = await _dbContextProvider.GetDbContextAsync();
                    if (!await dbContext.Database.CanConnectAsync(cancellationToken))
                    {
                        return HealthCheckResult.Unhealthy( "无法连接到数据库" );
                    }

                    var user = await dbContext.Users.AnyAsync(cancellationToken);
                    await uow.CompleteAsync();
                    if (user)
                    {
                        return HealthCheckResult.Healthy("已连接数据库，存在用户信息");
                    }

                    return HealthCheckResult.Unhealthy("已连接到数据库，不存在用户信息.");

                }
            }
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("无法连接到数据库。", e);
        }
    }
}

