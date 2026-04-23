using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTO.DCloud.DynamicEntityPropertyValues.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Entities;
using ADTOSharp.DynamicEntityProperties;
using ADTOSharp.Extensions;
using ADTOSharp.Collections.Extensions;


namespace ADTO.DCloud.DynamicEntityProperties;

/// <summary>
/// 动态实体属性值服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue)]
public class DynamicEntityPropertyValueAppService : DCloudAppServiceBase, IDynamicEntityPropertyValueAppService
{
    #region Fields
    private readonly IDynamicEntityPropertyValueManager _dynamicEntityPropertyValueManager;
    private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;
    private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;
    private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;
    #endregion
    #region Ctor
    public DynamicEntityPropertyValueAppService(
        IDynamicEntityPropertyValueManager dynamicEntityPropertyValueManager,
        IDynamicPropertyValueManager dynamicPropertyValueManager,
        IDynamicEntityPropertyManager dynamicEntityPropertyManager,
        IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager)
    {
        _dynamicEntityPropertyValueManager = dynamicEntityPropertyValueManager;
        _dynamicPropertyValueManager = dynamicPropertyValueManager;
        _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
        _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;
    }
    #endregion


    #region Utilities
    #endregion

    #region Methods

    /// <summary>
    /// 按ID获取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<DynamicEntityPropertyValueDto> Get(Guid id)
    {
        var entity = await _dynamicEntityPropertyValueManager.GetAsync(id);
        return ObjectMapper.Map<DynamicEntityPropertyValueDto>(entity);
    }

    /// <summary>
    /// 按属性+实体Id,获取列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ListResultDto<DynamicEntityPropertyValueDto>> GetAll(GetAllInput input)
    {
        var entities = await _dynamicEntityPropertyValueManager.GetValuesAsync(input.PropertyId, input.EntityId);
        return new ListResultDto<DynamicEntityPropertyValueDto>(
            ObjectMapper.Map<List<DynamicEntityPropertyValueDto>>(entities)
        );
    }
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Create)]
    public async Task Add(DynamicEntityPropertyValueDto input)
    {
        var entity = ObjectMapper.Map<DynamicEntityPropertyValue>(input);
        entity.TenantId = ADTOSharpSession.TenantId;
        await _dynamicEntityPropertyValueManager.AddAsync(entity);
    }
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Edit)]
    public async Task Update(DynamicEntityPropertyValueDto input)
    {
        var entity = await _dynamicEntityPropertyValueManager.GetAsync(input.Id);
        if (entity == null || entity.TenantId != ADTOSharpSession.TenantId)
        {
            throw new EntityNotFoundException(typeof(DynamicEntityPropertyValue), input.Id);
        }

        entity.Value = input.Value;
        entity.DynamicEntityPropertyId = input.DynamicEntityPropertyId;
        entity.EntityId = input.EntityId;

        await _dynamicEntityPropertyValueManager.UpdateAsync(entity);
    }
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Delete)]
    public async Task Delete(Guid id)
    {
        await _dynamicEntityPropertyValueManager.DeleteAsync(id);
    }
    /// <summary>
    /// 获取所有的动态属性值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<GetAllDynamicEntityPropertyValuesOutput> GetAllDynamicEntityPropertyValues(GetAllDynamicEntityPropertyValuesInput input)
    {
        var localCacheOfDynamicPropertyValues = new Dictionary<Guid, List<string>>();

        async Task<List<string>> LocalGetAllValuesOfDynamicProperty(Guid dynamicPropertyId)
        {
            if (!localCacheOfDynamicPropertyValues.ContainsKey(dynamicPropertyId))
            {
                localCacheOfDynamicPropertyValues[dynamicPropertyId] = (await _dynamicPropertyValueManager
                        .GetAllValuesOfDynamicPropertyAsync(dynamicPropertyId))
                    .Select(x => x.Value).ToList();
            }

            return localCacheOfDynamicPropertyValues[dynamicPropertyId];
        }

        var output = new GetAllDynamicEntityPropertyValuesOutput();
        var dynamicEntityProperties = await _dynamicEntityPropertyManager.GetAllAsync(input.EntityFullName);

        var dynamicEntityPropertySelectedValues = (await _dynamicEntityPropertyValueManager.GetValuesAsync(input.EntityFullName, input.EntityId))
            .GroupBy(value => value.DynamicEntityPropertyId)
            .ToDictionary(
                group => group.Key,
                items => items.ToList().Select(value => value.Value)
                    .ToList()
            );

        foreach (var dynamicEntityProperty in dynamicEntityProperties)
        {
            var outputItem = new GetAllDynamicEntityPropertyValuesOutputItem
            {
                DynamicEntityPropertyId = dynamicEntityProperty.Id,
                InputType = _dynamicEntityPropertyDefinitionManager.GetOrNullAllowedInputType(dynamicEntityProperty.DynamicProperty.InputType),
                PropertyName = dynamicEntityProperty.DynamicProperty.PropertyName,
                AllValuesInputTypeHas = await LocalGetAllValuesOfDynamicProperty(dynamicEntityProperty.DynamicProperty.Id),
                SelectedValues = dynamicEntityPropertySelectedValues.ContainsKey(dynamicEntityProperty.Id)
                    ? dynamicEntityPropertySelectedValues[dynamicEntityProperty.Id]
                    : new List<string>()
            };

            output.Items.Add(outputItem);
        }

        return output;
    }

    /// <summary>
    /// 新增/修改属性值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Create)]
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Edit)]
    public async Task InsertOrUpdateAllValues(InsertOrUpdateAllValuesInput input)
    {
        if (input.Items.IsNullOrEmpty())
        {
            return;
        }

        foreach (var item in input.Items)
        {
            await _dynamicEntityPropertyValueManager.CleanValuesAsync(item.DynamicEntityPropertyId, item.EntityId);

            foreach (var newValue in item.Values)
            {
                await _dynamicEntityPropertyValueManager.AddAsync(new DynamicEntityPropertyValue
                {
                    DynamicEntityPropertyId = item.DynamicEntityPropertyId,
                    EntityId = item.EntityId,
                    Value = newValue,
                    TenantId = ADTOSharpSession.TenantId
                });
            }
        }
    }
    /// <summary>
    /// 清除指定实体,指定属性的所有值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityPropertyValue_Delete)]
    public async Task CleanValues(CleanValuesInput input)
    {
        await _dynamicEntityPropertyValueManager.CleanValuesAsync(input.DynamicEntityPropertyId, input.EntityId);
    }
    #endregion
}
