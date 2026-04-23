using ADTOSharp.MultiTenancy;
using ADTOSharp.Zero.Configuration;

namespace ADTO.DCloud.Authorization.Roles;

/// <summary>
/// 应用角色的配置
/// </summary>
public static class AppRoleConfig
{
    public static void Configure(IRoleManagementConfig roleManagementConfig)
    {
        // 主机相关角色

        // 管理员
        roleManagementConfig.StaticRoles.Add(
            new StaticRoleDefinition(
                StaticRoleNames.Host.Admin,
                MultiTenancySides.Host,
                grantAllPermissionsByDefault: true)
            );

        //租户相关角色

        // 管理员
        roleManagementConfig.StaticRoles.Add(
            new StaticRoleDefinition(
                StaticRoleNames.Tenants.Admin,
                MultiTenancySides.Tenant,
                grantAllPermissionsByDefault: true)
            );

        // 用户
        roleManagementConfig.StaticRoles.Add(
            new StaticRoleDefinition(
                StaticRoleNames.Tenants.User,
                MultiTenancySides.Tenant)
            );


        //// Static host roles

        //roleManagementConfig.StaticRoles.Add(
        //    new StaticRoleDefinition(
        //        StaticRoleNames.Host.Admin,
        //        MultiTenancySides.Host
        //    )
        //);

        //// Static tenant roles

        //roleManagementConfig.StaticRoles.Add(
        //    new StaticRoleDefinition(
        //        StaticRoleNames.Tenants.Admin,
        //        MultiTenancySides.Tenant
        //    )
        //);
    }
}
