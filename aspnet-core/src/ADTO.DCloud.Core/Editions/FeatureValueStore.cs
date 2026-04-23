using ADTOSharp.Application.Features;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Caching;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using System;


namespace ADTO.DCloud.Editions;

public class FeatureValueStore : ADTOSharpFeatureValueStore<Tenant, User>
{
    public FeatureValueStore(
        ICacheManager cacheManager,
        IRepository<TenantFeatureSetting, Guid> tenantFeatureSettingRepository,
        IRepository<Tenant, Guid> tenantRepository,
        IRepository<EditionFeatureSetting, Guid> editionFeatureSettingRepository,
        IFeatureManager featureManager,
        IUnitOfWorkManager unitOfWorkManager)
        : base(cacheManager,
              tenantFeatureSettingRepository,
              tenantRepository,
              editionFeatureSettingRepository,
              featureManager,
              unitOfWorkManager)
    {
    }
}
