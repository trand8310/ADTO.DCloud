using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.DynamicEntityProperties;
using ADTOSharp.UI.Inputs;

namespace ADTO.DCloud.DynamicEntityProperties;

/// <summary>
/// 动态属性服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicProperties)]
public class DynamicPropertyAppService : DCloudAppServiceBase, IDynamicPropertyAppService
{
    #region Fields
    private readonly IDynamicPropertyManager _dynamicPropertyManager;
    private readonly IDynamicPropertyStore _dynamicPropertyStore;
    private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;
    #endregion
    #region Ctor
    public DynamicPropertyAppService(
        IDynamicPropertyManager dynamicPropertyManager,
        IDynamicPropertyStore dynamicPropertyStore,
        IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager)
    {
        _dynamicPropertyManager = dynamicPropertyManager;
        _dynamicPropertyStore = dynamicPropertyStore;
        _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;
    }
    #endregion


    #region Utilities
    #endregion

    #region Methods
    /// <summary>
    /// 依ID获取属性
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<DynamicPropertyDto> Get(Guid id)
    {
        var entity = await _dynamicPropertyManager.GetAsync(id);
        return ObjectMapper.Map<DynamicPropertyDto>(entity);
    }
    /// <summary>
    /// 获取所有属性
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<DynamicPropertyDto>> GetAll()
    {
        var entities = await _dynamicPropertyStore.GetAllAsync();

        return new ListResultDto<DynamicPropertyDto>(
            ObjectMapper.Map<List<DynamicPropertyDto>>(entities)
        );
    }
    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue_Create)]
    public async Task Add(DynamicPropertyDto dto)
    {
        dto.TenantId = ADTOSharpSession.TenantId;
        await _dynamicPropertyManager.AddAsync(ObjectMapper.Map<DynamicProperty>(dto));
    }
    /// <summary>
    /// 修改属性
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue_Edit)]
    public async Task Update(DynamicPropertyDto dto)
    {
        dto.TenantId = ADTOSharpSession.TenantId;
        await _dynamicPropertyManager.UpdateAsync(ObjectMapper.Map<DynamicProperty>(dto));
    }
    /// <summary>
    /// 删除属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicPropertyValue_Delete)]
    public async Task Delete(EntityDto<Guid> input)
    {
        await _dynamicPropertyManager.DeleteAsync(input.Id);
    }
    
    /// <summary>
    /// 获取指定属性能使用的输入类型
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>

    public IInputType FindAllowedInputType(string name)
    {
        return _dynamicEntityPropertyDefinitionManager.GetOrNullAllowedInputType(name);
    }
    #endregion
}

