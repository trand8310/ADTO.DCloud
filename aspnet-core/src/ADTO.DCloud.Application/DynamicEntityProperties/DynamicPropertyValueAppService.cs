using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.DynamicEntityProperties;

namespace ADTO.DCloud.DynamicEntityProperties;

/// <summary>
/// 动态属性值服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue)]
public class DynamicPropertyValueAppService : DCloudAppServiceBase, IDynamicPropertyValueAppService
{
    private readonly IDynamicPropertyValueManager _dynamicPropertyValueManager;
    private readonly IDynamicPropertyValueStore _dynamicPropertyValueStore;

    public DynamicPropertyValueAppService(
        IDynamicPropertyValueManager dynamicPropertyValueManager,
        IDynamicPropertyValueStore dynamicPropertyValueStore
    )
    {
        _dynamicPropertyValueManager = dynamicPropertyValueManager;
        _dynamicPropertyValueStore = dynamicPropertyValueStore;
    }
    /// <summary>
    /// 依ID获取属性值
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<DynamicPropertyValueDto> Get(Guid id)
    {
        var entity = await _dynamicPropertyValueManager.GetAsync(id);
        return ObjectMapper.Map<DynamicPropertyValueDto>(entity);
    }

    /// <summary>
    /// 获取指定属性的所有值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<ListResultDto<DynamicPropertyValueDto>> GetAllValuesOfDynamicProperty(EntityDto<Guid> input)
    {
        var entities = await _dynamicPropertyValueStore.GetAllValuesOfDynamicPropertyAsync(input.Id);
        return new ListResultDto<DynamicPropertyValueDto>(
            ObjectMapper.Map<List<DynamicPropertyValueDto>>(entities)
        );
    }
    /// <summary>
    /// 添加属性值
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue_Create)]
    public async Task Add(DynamicPropertyValueDto dto)
    {
        dto.TenantId = ADTOSharpSession.TenantId;
        await _dynamicPropertyValueManager.AddAsync(ObjectMapper.Map<DynamicPropertyValue>(dto));
    }
    /// <summary>
    /// 修改属性值
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue_Edit)]
    public async Task Update(DynamicPropertyValueDto dto)
    {
        dto.TenantId = ADTOSharpSession.TenantId;
        await _dynamicPropertyValueManager.UpdateAsync(ObjectMapper.Map<DynamicPropertyValue>(dto));
    }
    /// <summary>
    /// 删除属性值
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue_Delete)]
    public async Task Delete(EntityDto<Guid> input)
    {
        await _dynamicPropertyValueManager.DeleteAsync(input.Id);
    }
}
