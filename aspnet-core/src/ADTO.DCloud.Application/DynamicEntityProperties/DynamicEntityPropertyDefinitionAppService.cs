using System.Collections.Generic;
using ADTO.DCloud.Authorization;
using ADTOSharp.Authorization;
using ADTOSharp.DynamicEntityProperties;

namespace ADTO.DCloud.DynamicEntityProperties;

/// <summary>
/// 动态实体属性定义服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_DynamicProperties)]
public class DynamicEntityPropertyDefinitionAppService : DCloudAppServiceBase, IDynamicEntityPropertyDefinitionAppService
{
    #region Fields
    private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;
    #endregion
    #region Ctor
    public DynamicEntityPropertyDefinitionAppService(IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager)
    {
        _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;
    }
    #endregion


    #region Utilities
    #endregion

    #region Methods
    /// <summary>
    /// 获取实体动态属性值支持的输入控件名称,INPUT,SELECT,CHECKBOX...
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllAllowedInputTypeNames()
    {
        return _dynamicEntityPropertyDefinitionManager.GetAllAllowedInputTypeNames();
    }
    /// <summary>
    /// 获取所有实体名称,这些名称都是支持动态添加属性的实体
    /// </summary>
    /// <returns></returns>
    public List<string> GetAllEntities()
    {
        return _dynamicEntityPropertyDefinitionManager.GetAllEntities();
    }
    #endregion
}

