using System;
using System.Diagnostics;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Threading;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Threading.Timers;
using ADTOSharp.Timing;
using JetBrains.Annotations;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Configuration;

namespace ADTO.DCloud.MultiTenancy
{
    public class SubscriptionExpireEmailNotifierWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
        
        private readonly IRepository<Tenant, Guid> _tenantRepository;
        private readonly UserEmailer _userEmailer;
        private readonly IUnitOfWorkManager _unitOfWorkManager; 

        public SubscriptionExpireEmailNotifierWorker(
            ADTOSharpTimer timer,
            IRepository<Tenant, Guid> tenantRepository,
            UserEmailer userEmailer, 
            IUnitOfWorkManager unitOfWorkManager) : base(timer)
        {
            _tenantRepository = tenantRepository;
            _userEmailer = userEmailer;
            _unitOfWorkManager = unitOfWorkManager;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;

            LocalizationSourceName = DCloudConsts.LocalizationSourceName;
        }

        protected override void DoWork()
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var subscriptionRemainingDayCount = Convert.ToInt32(SettingManager.GetSettingValueForApplication(AppSettings.TenantManagement.SubscriptionExpireNotifyDayCount));
                var dateToCheckRemainingDayCount = Clock.Now.AddDays(subscriptionRemainingDayCount).ToUniversalTime();

                var subscriptionExpiredTenants = _tenantRepository.GetAllList(
                    tenant => tenant.SubscriptionEndDateUtc != null &&
                              tenant.SubscriptionEndDateUtc.Value.Date == dateToCheckRemainingDayCount.Date &&
                              tenant.IsActive &&
                              tenant.EditionId != null
                );

                foreach (var tenant in subscriptionExpiredTenants)
                {
                    Debug.Assert(tenant.EditionId.HasValue);
                    try
                    {
                        AsyncHelper.RunSync(() => _userEmailer.TryToSendSubscriptionExpiringSoonEmail(tenant.Id, dateToCheckRemainingDayCount));
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message, exception);
                    }
                }
            });
        }
    }
}

