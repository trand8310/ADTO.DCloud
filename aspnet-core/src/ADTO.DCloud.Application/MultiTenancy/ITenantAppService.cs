using ADTOSharp.Application.Services;
using ADTO.DCloud.MultiTenancy.Dto;
using System;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ADTO.DCloud.MultiTenancy;

public interface ITenantAppService : IApplicationService
{
    Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsDto input);

    Task CreateTenant(CreateTenantDto input);

    Task<TenantEditDto> GetTenantForEdit(EntityDto<Guid> input);

    Task UpdateTenant(TenantEditDto input);

    Task DeleteTenant(EntityDto<Guid> input);

    Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto<Guid> input);

    Task UpdateTenantFeatures(UpdateTenantFeaturesInput input);

    Task ResetTenantSpecificFeatures(EntityDto<Guid> input);

    Task UnlockTenantAdmin(EntityDto<Guid> input);
}

