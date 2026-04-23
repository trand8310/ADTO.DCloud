using ADTOSharp.BackgroundJobs;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using System;
using System.Threading.Tasks;



namespace ADTO.DCloud.MultiTenancy.Demo
{
    public class TenantDemoDataBuilderJob : AsyncBackgroundJob<Guid>, ITransientDependency
    {
        private readonly TenantDemoDataBuilder _tenantDemoDataBuilder;
        private readonly TenantManager _tenantManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantDemoDataBuilderJob(
            TenantDemoDataBuilder tenantDemoDataBuilder, 
            TenantManager tenantManager, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantDemoDataBuilder = tenantDemoDataBuilder;
            _tenantManager = tenantManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task ExecuteAsync(Guid args)
        {
            var tenantId = args;
            var tenant = await _tenantManager.GetByIdAsync(tenantId);
            using (var uow = _unitOfWorkManager.Begin())
            {
                await _tenantDemoDataBuilder.BuildForAsync(tenant);
                await uow.CompleteAsync();
            }
        }
    }
}
