using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.DataAuthorizes;
using ADTO.DCloud.MultiTenancy;
using ADTOSharp.Application.Services;
using ADTOSharp.IdentityFramework;
using ADTOSharp.Runtime.Session;
using Microsoft.AspNetCore.Identity;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ADTO.DCloud;

/// <summary>
/// 应用程序服务基类
/// 所有的应用程序服务都需要从这个类派生。
/// </summary>
public abstract class DCloudAppServiceBase : ApplicationService
{
    public TenantManager TenantManager { get; set; }

    public UserManager UserManager { get; set; }

    protected DCloudAppServiceBase()
    {
        LocalizationSourceName = DCloudConsts.LocalizationSourceName;
    }
    /// <summary>
    /// 当前用户
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    protected virtual async Task<User> GetCurrentUserAsync()
    {
        var user = await UserManager.FindByIdAsync(ADTOSharpSession.GetUserId().ToString());
        if (user == null)
        {
            throw new Exception("无效的用户信息！");
        }

        return user;
    }

    /// <summary>
    /// 当前租户
    /// </summary>
    /// <returns></returns>
    protected virtual Task<Tenant> GetCurrentTenantAsync()
    {
        return TenantManager.GetByIdAsync(ADTOSharpSession.GetTenantId());
    }

    /// <summary>
    /// 错误检测,可以请求的消息以本地化的方式抛出给用户
    /// </summary>
    /// <param name="identityResult"></param>
    protected virtual void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }

    protected virtual string GetCurrentPermissionCode([CallerMemberName] string methodName = "")
    {
        var method = this.GetType().GetMethod(methodName);
        return PermissionHelper.GetPermissionCode(method);
    }
}

