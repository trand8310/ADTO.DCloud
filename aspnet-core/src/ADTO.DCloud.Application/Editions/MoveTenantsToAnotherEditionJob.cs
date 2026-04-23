using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Notifications;

namespace ADTO.DCloud.Editions
{
    public class MoveTenantsToAnotherEditionJob : AsyncBackgroundJob<MoveTenantsToAnotherEditionJobArgs>, ITransientDependency
    {
        private readonly IRepository<Tenant,Guid> _tenantRepository;
        private readonly EditionManager _editionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        
        public IEventBus EventBus { get; set; }

        public MoveTenantsToAnotherEditionJob(
            IRepository<Tenant, Guid> tenantRepository,
            EditionManager editionManager,
            IAppNotifier appNotifier,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantRepository = tenantRepository;
            _editionManager = editionManager;
            _appNotifier = appNotifier;
            _unitOfWorkManager = unitOfWorkManager;

            EventBus = NullEventBus.Instance;
        }

        public override async Task ExecuteAsync(MoveTenantsToAnotherEditionJobArgs args)
        {
            if (args.SourceEditionId == args.TargetEditionId)
            {
                return;
            }

            List<Guid> tenantIds;

            using (var uow = _unitOfWorkManager.Begin())
            {
                tenantIds = _tenantRepository.GetAll()
                    .Where(t => t.EditionId == args.SourceEditionId)
                    .Select(t => t.Id)
                    .ToList();
                
                await uow.CompleteAsync();
            }

            if (!tenantIds.Any())
            {
                return;
            }

            var changedTenantCount = await ChangeEditionOfTenantsAsync(tenantIds, args.SourceEditionId, args.TargetEditionId);

            if (changedTenantCount != tenantIds.Count)
            {
                Logger.Warn($"Unable to move all tenants from edition {args.SourceEditionId} to edition {args.TargetEditionId}");
                return;
            }

            await NotifyUserAsync(args);
        }

        private async Task<int> ChangeEditionOfTenantsAsync(List<Guid> tenantIds, Guid sourceEditionId, Guid targetEditionId)
        {
            var changedTenantCount = 0;

            foreach (var tenantId in tenantIds)
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    var changed = await ChangeEditionOfTenantAsync(tenantId, sourceEditionId, targetEditionId);
                    if (changed)
                    {
                        changedTenantCount++;
                    }

                    await uow.CompleteAsync();
                }
            }

            return changedTenantCount;
        }

        private async Task NotifyUserAsync(MoveTenantsToAnotherEditionJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var sourceEdition = await _editionManager.GetByIdAsync(args.SourceEditionId);
                var targetEdition = await _editionManager.GetByIdAsync(args.TargetEditionId);

                await _appNotifier.TenantsMovedToEdition(
                    args.User,
                    sourceEdition.DisplayName,
                    targetEdition.DisplayName
                );

                await uow.CompleteAsync();
            }
        }

        private async Task<bool> ChangeEditionOfTenantAsync(Guid tenantId, Guid sourceEditionId, Guid targetEditionId)
        {
            try
            {
                var tenant = await _tenantRepository.GetAsync(tenantId);
                tenant.EditionId = targetEditionId;

                await CurrentUnitOfWork.SaveChangesAsync();

                await EventBus.TriggerAsync(new TenantEditionChangedEventData
                {
                    TenantId = tenant.Id,
                    OldEditionId = sourceEditionId,
                    NewEditionId = targetEditionId
                });

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                return false;
            }
        }
    }
}
