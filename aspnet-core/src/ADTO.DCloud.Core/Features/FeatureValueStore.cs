using ADTOSharp.Application.Features;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Caching;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using System;

namespace ADTO.DCloud.Features
{
    public class FeatureValueStore : ADTOSharpFeatureValueStore<Tenant, User>
    {
        public FeatureValueStore(
            ICacheManager cacheManager, 
            IRepository<TenantFeatureSetting, Guid> tenantFeatureRepository, 
            IRepository<Tenant, Guid> tenantRepository, 
            IRepository<EditionFeatureSetting, Guid> editionFeatureRepository, 
            IFeatureManager featureManager, 
            IUnitOfWorkManager unitOfWorkManager) 
            : base(
                  cacheManager, 
                  tenantFeatureRepository, 
                  tenantRepository, 
                  editionFeatureRepository, 
                  featureManager, 
                  unitOfWorkManager)
        {
        }
    }
}
