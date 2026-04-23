using System;
using System.Collections.Generic;
using System.Linq;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Threading.Timers;
using ADTOSharp.Timing;

namespace ADTOSharp.Authorization.Users;

public class UserTokenExpirationWorker<TTenant, TUser> : PeriodicBackgroundWorkerBase
    where TTenant : ADTOSharpTenant<TUser>
    where TUser : ADTOSharpUserBase
{
    private readonly IRepository<UserToken, Guid> _userTokenRepository;
    private readonly IRepository<TTenant,Guid> _tenantRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UserTokenExpirationWorker(
        ADTOSharpTimer timer,
        IRepository<UserToken, Guid> userTokenRepository,
        IBackgroundJobConfiguration backgroundJobConfiguration,
        IUnitOfWorkManager unitOfWorkManager,
        IRepository<TTenant, Guid> tenantRepository)
        : base(timer)
    {
        _userTokenRepository = userTokenRepository;
        _unitOfWorkManager = unitOfWorkManager;
        _tenantRepository = tenantRepository;

        Timer.Period = backgroundJobConfiguration.UserTokenExpirationPeriod?.TotalMilliseconds.To<int>()
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once support for CleanUserTokenPeriod property is removed
                       ?? backgroundJobConfiguration.CleanUserTokenPeriod
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once support for CleanUserTokenPeriod property is removed
                       ?? TimeSpan.FromHours(1).TotalMilliseconds.To<int>();
    }

    protected override void DoWork()
    {
        List<Guid> tenantIds;
        var utcNow = Clock.Now.ToUniversalTime();

        using (var uow = _unitOfWorkManager.Begin())
        {
            using (_unitOfWorkManager.Current.SetTenantId(null))
            {
                _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                tenantIds = _tenantRepository.GetAll().Select(t => t.Id).ToList();
                uow.Complete();
            }
        }

        foreach (var tenantId in tenantIds)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    _userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
                    uow.Complete();
                }
            }
        }
    }
}