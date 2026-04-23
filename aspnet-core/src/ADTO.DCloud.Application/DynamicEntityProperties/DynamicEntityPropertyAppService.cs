using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.DynamicEntityProperties.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.DynamicEntityProperties;

namespace ADTO.DCloud.DynamicEntityProperties;

/// <summary>
/// 动态实体属性服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityProperties)]
public class DynamicEntityPropertyAppService : DCloudAppServiceBase, IDynamicEntityPropertyAppService
{
    #region Fields
    private readonly IDynamicEntityPropertyManager _dynamicEntityPropertyManager;
    #endregion

    #region Ctor
    public DynamicEntityPropertyAppService(IDynamicEntityPropertyManager dynamicEntityPropertyManager)
    {
        _dynamicEntityPropertyManager = dynamicEntityPropertyManager;
    }
    #endregion

    #region Utilities
    #endregion

    #region Methods
    /// <summary>
    /// 获取一个动态属性对像
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<DynamicEntityPropertyDto> Get(EntityDto<Guid> input)
    {
        var entity = await _dynamicEntityPropertyManager.GetAsync(input.Id);
        return ObjectMapper.Map<DynamicEntityPropertyDto>(entity);
    }
    /// <summary>
    /// 获取一个实体的所有动态属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public async Task<ListResultDto<DynamicEntityPropertyDto>> GetAllPropertiesOfAnEntity(DynamicEntityPropertyGetAllInput input)
    {
        var entities = await _dynamicEntityPropertyManager.GetAllAsync(input.EntityFullName);
        return new ListResultDto<DynamicEntityPropertyDto>(
            ObjectMapper.Map<List<DynamicEntityPropertyDto>>(entities)
        );
    }

    /// <summary>
    /// 获取所有的动态属性,一般用于像下拉选择.
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<DynamicEntityPropertyDto>> GetAll()
    {
        var entities = await _dynamicEntityPropertyManager.GetAllAsync();
        var result = entities.Select(item =>
        {
            var dto = ObjectMapper.Map<DynamicEntityPropertyDto>(item);  
            dto.DynamicPropertyName = item.DynamicProperty?.PropertyName;  
            return dto;
        }).ToList();

        return new ListResultDto<DynamicEntityPropertyDto>(result);

        //return new ListResultDto<DynamicEntityPropertyDto>(
        //    ObjectMapper.Map<List<DynamicEntityPropertyDto>>(entities)
        //);
    }

    /// <summary>
    /// 创建动态属性
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityProperties_Create)]
    public async Task Add(DynamicEntityPropertyDto dto)
    {
        
        dto.TenantId = ADTOSharpSession.TenantId;
        await _dynamicEntityPropertyManager.AddAsync(ObjectMapper.Map<DynamicEntityProperty>(dto));
    }

    /// <summary>
    /// 修改动态属性
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityProperties_Edit)]
    public async Task Update(DynamicEntityPropertyDto dto)
    {
        await _dynamicEntityPropertyManager.UpdateAsync(ObjectMapper.Map<DynamicEntityProperty>(dto));
    }
    /// <summary>
    /// 删除动态属性
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicEntityProperties_Delete)]
    public async Task Delete(EntityDto<Guid> input)
    {
        await _dynamicEntityPropertyManager.DeleteAsync(input.Id);
    }
    /// <summary>
    /// 获取所有存在动态属性的实体列表
    /// </summary>
    /// <returns></returns>
    public async Task<ListResultDto<GetAllEntitiesHasDynamicPropertyOutput>> GetAllEntitiesHasDynamicProperty()
    {
        var entities = await _dynamicEntityPropertyManager.GetAllAsync();
        return new ListResultDto<GetAllEntitiesHasDynamicPropertyOutput>(
            entities?.Select(x => new GetAllEntitiesHasDynamicPropertyOutput()
            {
                EntityFullName = x.EntityFullName
            }).DistinctBy(x => x.EntityFullName).ToList()
        );
    }
    #endregion
}

