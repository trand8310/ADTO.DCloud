using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Application.Editions;
using ADTOSharp.Application.Features;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Editions.Dto;
using ADTO.DCloud.MultiTenancy;
using System;

namespace ADTO.DCloud.Editions;
/// <summary>
/// 版本管理服务
/// </summary>

public class EditionAppService : DCloudAppServiceBase, IEditionAppService
{
    private readonly EditionManager _editionManager;
    private readonly IRepository<SubscribableEdition,Guid> _editionRepository;
    private readonly IRepository<Tenant,Guid> _tenantRepository;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public EditionAppService(
        EditionManager editionManager,
        IRepository<SubscribableEdition, Guid> editionRepository,
        IRepository<Tenant,Guid> tenantRepository,
        IBackgroundJobManager backgroundJobManager)
    {
        _editionManager = editionManager;
        _editionRepository = editionRepository;
        _tenantRepository = tenantRepository;
        _backgroundJobManager = backgroundJobManager;
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions)]
    public async Task<ListResultDto<EditionListDto>> GetEditions()
    {
        var editions = await (from edition in _editionRepository.GetAll()
                              join expiringEdition in _editionRepository.GetAll() on edition.ExpiringEditionId equals expiringEdition.Id into expiringEditionJoined
                              from expiringEdition in expiringEditionJoined.DefaultIfEmpty()
                              select new
                              {
                                  Edition = edition,
                                  expiringEditionDisplayName = expiringEdition.DisplayName
                              }).ToListAsync();

        var result = new List<EditionListDto>();

        foreach (var edition in editions)
        {
            var resultEdition = ObjectMapper.Map<EditionListDto>(edition.Edition);
            resultEdition.ExpiringEditionDisplayName = edition.expiringEditionDisplayName;

            result.Add(resultEdition);
        }

        return new ListResultDto<EditionListDto>(result);
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_Create, PermissionNames.Pages_Editions_Edit)]
    public async Task<GetEditionEditOutput> GetEditionForEdit(NullableIdDto<Guid> input)
    {
        var features = FeatureManager.GetAll()
            .Where(f => f.Scope.HasFlag(FeatureScopes.Edition));

        EditionEditDto editionEditDto;
        List<NameValue> featureValues;

        if (input.Id.HasValue) //Editing existing edition?
        {
            var edition = await _editionManager.FindByIdAsync(input.Id.Value);
            featureValues = (await _editionManager.GetFeatureValuesAsync(input.Id.Value)).ToList();
            editionEditDto = ObjectMapper.Map<EditionEditDto>(edition);
        }
        else
        {
            editionEditDto = new EditionEditDto();
            featureValues = features.Select(f => new NameValue(f.Name, f.DefaultValue)).ToList();
        }

        var featureDtos = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList();

        return new GetEditionEditOutput
        {
            Edition = editionEditDto,
            Features = featureDtos,
            FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
        };
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_Create)]
    public async Task CreateEdition(CreateEditionDto input)
    {
        await CreateEditionAsync(input);
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_Edit)]
    public async Task UpdateEdition(UpdateEditionDto input)
    {
        await UpdateEditionAsync(input);
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_Delete)]
    public async Task DeleteEdition(EntityDto<Guid> input)
    {
        var tenantCount = await _tenantRepository.CountAsync(t => t.EditionId == input.Id);
        if (tenantCount > 0)
        {
            throw new UserFriendlyException(L("ThereAreTenantsSubscribedToThisEdition"));
        }

        var edition = await _editionManager.GetByIdAsync(input.Id);
        await _editionManager.DeleteAsync(edition);
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_MoveTenantsToAnotherEdition)]
    public async Task MoveTenantsToAnotherEdition(MoveTenantsToAnotherEditionDto input)
    {
        await _backgroundJobManager.EnqueueAsync<MoveTenantsToAnotherEditionJob, MoveTenantsToAnotherEditionJobArgs>(new MoveTenantsToAnotherEditionJobArgs
        {
            SourceEditionId = input.SourceEditionId,
            TargetEditionId = input.TargetEditionId,
            User = ADTOSharpSession.ToUserIdentifier()
        });
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions,PermissionNames.Pages_Tenants)]
    public async Task<List<SubscribableEditionComboboxItemDto>> GetEditionComboboxItems(int? selectedEditionId = null, bool addAllItem = false, bool onlyFreeItems = false)
    {
        var editions = await _editionManager.Editions.ToListAsync();
        var subscribableEditions = editions.Cast<SubscribableEdition>()
            .WhereIf(onlyFreeItems, e => e.IsFree)
            .OrderBy(e => e.MonthlyPrice);

        var editionItems =
            new ListResultDto<SubscribableEditionComboboxItemDto>(subscribableEditions
                .Select(e => new SubscribableEditionComboboxItemDto(e.Id.ToString(), e.DisplayName, e.IsFree)).ToList()).Items.ToList();

        var defaultItem = new SubscribableEditionComboboxItemDto("", L("NotAssigned"), null);
        editionItems.Insert(0, defaultItem);

        if (addAllItem)
        {
            editionItems.Insert(0, new SubscribableEditionComboboxItemDto("-1", "- " + L("All") + " -", null));
        }

        if (selectedEditionId.HasValue)
        {
            var selectedEdition = editionItems.FirstOrDefault(e => e.Value == selectedEditionId.Value.ToString());
            if (selectedEdition != null)
            {
                selectedEdition.IsSelected = true;
            }
        }
        else
        {
            editionItems[0].IsSelected = true;
        }

        return editionItems;
    }

    public async Task<int> GetTenantCount(Guid editionId)
    {
        return await _tenantRepository.CountAsync(t => t.EditionId == editionId);
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_Create)]
    protected virtual async Task CreateEditionAsync(CreateEditionDto input)
    {
        var edition = ObjectMapper.Map<SubscribableEdition>(input.Edition);

        if (edition.ExpiringEditionId.HasValue)
        {
            var expiringEdition = (SubscribableEdition)await _editionManager.GetByIdAsync(edition.ExpiringEditionId.Value);
            if (!expiringEdition.IsFree)
            {
                throw new UserFriendlyException(L("ExpiringEditionMustBeAFreeEdition"));
            }
        }

        await _editionManager.CreateAsync(edition);
        await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the edition.

        await SetFeatureValues(edition, input.FeatureValues);
    }

    [ADTOSharpAuthorize(PermissionNames.Pages_Editions_Edit)]
    protected virtual async Task UpdateEditionAsync(UpdateEditionDto input)
    {
        if (input.Edition.Id != null)
        {
            var edition = await _editionManager.GetByIdAsync(input.Edition.Id.Value);

            edition.DisplayName = input.Edition.DisplayName;

            await SetFeatureValues(edition, input.FeatureValues);
        }
    }

    private Task SetFeatureValues(Edition edition, List<NameValueDto> featureValues)
    {
        return _editionManager.SetFeatureValuesAsync(edition.Id,
            featureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
    }
}

