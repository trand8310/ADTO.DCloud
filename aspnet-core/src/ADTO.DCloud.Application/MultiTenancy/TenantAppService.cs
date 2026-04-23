using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Security;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Editions;
using ADTO.DCloud.MultiTenancy.Dto;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System;
using ADTOSharp.Events.Bus;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ADTOSharp.Domain.Uow;
using ADTO.DCloud.Url;
using ADTOSharp.Application.Features;
using ADTOSharp;
using ADTO.DCloud.Editions.Dto;


namespace ADTO.DCloud.MultiTenancy;

/// <summary>
/// 租户服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Tenants)]
public class TenantAppService : DCloudAppServiceBase, ITenantAppService
{
    #region 
    private readonly IRepository<Tenant, Guid> _tenatntRepository;
    private readonly TenantManager _tenantManager;
    private readonly EditionManager _editionManager;
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IADTOSharpZeroDbMigrator _adtoZeroDbMigrator;
    public IAppUrlService AppUrlService { get; set; }
    public IEventBus EventBus { get; set; }

    public TenantAppService(
                IRepository<Tenant, Guid> tenatntRepository,
                TenantManager tenantManager,
                EditionManager editionManager,
                UserManager userManager,
                RoleManager roleManager,
                IADTOSharpZeroDbMigrator adtoZeroDbMigrator)
    {
        _tenatntRepository = tenatntRepository;
        _tenantManager = tenantManager;
        _editionManager = editionManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _adtoZeroDbMigrator = adtoZeroDbMigrator;
        AppUrlService = NullAppUrlService.Instance;
        EventBus = NullEventBus.Instance;
    }
    #endregion

    /// <summary>
    /// 获取租户分页列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsDto input)
    {
        var query = TenantManager.Tenants
            .Include(t => t.Edition)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Keyword) || t.TenancyName.Contains(input.Keyword))
            .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
            .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value)
            .WhereIf(input.SubscriptionEndDateStart.HasValue, t => t.SubscriptionEndDateUtc >= input.SubscriptionEndDateStart.Value.ToUniversalTime())
            .WhereIf(input.SubscriptionEndDateEnd.HasValue, t => t.SubscriptionEndDateUtc <= input.SubscriptionEndDateEnd.Value.ToUniversalTime())
            .WhereIf(input.EditionIdSpecified, t => t.EditionId == input.EditionId);

        var tenantCount = await query.CountAsync();
        var tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
        var list = ObjectMapper.Map<List<TenantListDto>>(tenants);
        return new PagedResultDto<TenantListDto>(
            tenantCount,
            list
        );
    }

    /// <summary>
    /// 创建租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork(IsDisabled = true)]
    public async Task CreateTenant(CreateTenantDto input)
    {
        try
        {
            await TenantManager.CreateWithAdminUserAsync(
           input.TenancyName,
           input.Name,
           input.AdminPassword,
           input.AdminEmailAddress,
           input.ConnectionString,
           input.IsActive,
           input.EditionId,
           input.ShouldChangePasswordOnNextLogin,
           input.SendActivationEmail,
           input.SubscriptionEndDateUtc?.ToUniversalTime(),
           input.IsInTrialPeriod,
           AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName),
           adminName: input.AdminName
       );
        }
        catch (Exception ex)
        {

            throw;
        }
       
    }

    /// <summary>
    /// 获取租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<TenantEditDto> GetTenantForEdit(EntityDto<Guid> input)
    {
        var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
        tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);
        return tenantEditDto;
    }

    /// <summary>
    /// 修改租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateTenant(TenantEditDto input)
    {
        await TenantManager.CheckEditionAsync(input.EditionId, input.IsInTrialPeriod);

        input.ConnectionString = SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
        var tenant = await TenantManager.GetByIdAsync(input.Id);

        if (tenant.EditionId != input.EditionId)
        {
            await EventBus.TriggerAsync(new TenantEditionChangedEventData
            {
                TenantId = input.Id,
                OldEditionId = tenant.EditionId,
                NewEditionId = input.EditionId
            });
        }

        ObjectMapper.Map(input, tenant);
        tenant.SubscriptionEndDateUtc = tenant.SubscriptionEndDateUtc?.ToUniversalTime();

        await TenantManager.UpdateAsync(tenant);
    }

    /// <summary>
    /// 删除租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeleteTenant(EntityDto<Guid> input)
    {
        var tenant = await TenantManager.GetByIdAsync(input.Id);
        await TenantManager.DeleteAsync(tenant);
    }

    #region 租户特性

    public async Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto<Guid> input)
    {
        var features = FeatureManager.GetAll()
            .Where(f => f.Scope.HasFlag(FeatureScopes.Tenant));
        var featureValues = await TenantManager.GetFeatureValuesAsync(input.Id);

        return new GetTenantFeaturesEditOutput
        {
            Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
            FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
        };
    }
    public async Task UpdateTenantFeatures(UpdateTenantFeaturesInput input)
    {
        await TenantManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
    }

    public async Task ResetTenantSpecificFeatures(EntityDto<Guid> input)
    {
        await TenantManager.ResetAllFeaturesAsync(input.Id);
    }

    #endregion

    public async Task UnlockTenantAdmin(EntityDto<Guid> input)
    {
        using (CurrentUnitOfWork.SetTenantId(input.Id))
        {
            var tenantAdmin = await UserManager.GetAdminAsync();
            if (tenantAdmin != null)
            {
                tenantAdmin.Unlock();
            }
        }
    }
}


