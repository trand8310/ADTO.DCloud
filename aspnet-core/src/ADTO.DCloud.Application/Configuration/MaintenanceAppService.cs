using ADTO.DCloud.Authorization;
using ADTO.DCloud.Configuration.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.UiCustomization;
using ADTOSharp.Authorization;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace ADTO.DCloud.Configuration;

/// <summary>
/// 维护服务,配置前端使用
/// </summary>
[ADTOSharpAuthorize]
public class MaintenanceAppService : DCloudAppServiceBase, IMaintenanceAppService
{

    private readonly IIocResolver _iocResolver;


    public MaintenanceAppService(
        SettingManager settingManager,
        IIocResolver iocResolver,
        IUiThemeCustomizerFactory uiThemeCustomizerFactory
    )
    {
        _iocResolver = iocResolver;
    }

    /// <summary>
    /// 重启应用系统
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpGet, AllowAnonymous]
    public bool RestartAppSystem()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "web.config");
            if (System.IO.File.Exists(path))
                System.IO.File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
            return true;
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(ex.Message);
        }

    }
}
