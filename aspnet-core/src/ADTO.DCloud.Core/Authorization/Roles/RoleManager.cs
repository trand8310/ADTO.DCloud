using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Zero.Configuration;
using ADTO.DCloud.Authorization.Users;
using System;
using ADTOSharp.Localization;
using ADTOSharp.UI;
using System.Linq;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Roles;

/// <summary>
/// 角色管理
/// </summary>
public class RoleManager : ADTOSharpRoleManager<Role, User>
{
    private readonly ILocalizationManager _localizationManager;

    public RoleManager(
        RoleStore store,
        IEnumerable<IRoleValidator<Role>> roleValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        ILogger<RoleManager> logger,
        IPermissionManager permissionManager,
        IRoleManagementConfig roleManagementConfig,
        ICacheManager cacheManager,
        IUnitOfWorkManager unitOfWorkManager,
        ILocalizationManager localizationManager,
        IRepository<OrganizationUnit, Guid> organizationUnitRepository,
        IRepository<OrganizationUnitRole, Guid> organizationUnitRoleRepository)
        : base(
            store,
            roleValidators,
            keyNormalizer,
            errors,
            logger,
            permissionManager,
            cacheManager,
            unitOfWorkManager,
            roleManagementConfig,
            organizationUnitRepository,
            organizationUnitRoleRepository)
    {
        _localizationManager = localizationManager;
    }

    public override Task SetGrantedPermissionsAsync(Role role, IEnumerable<Permission> permissions)
    {
        CheckPermissionsToUpdate(role, permissions);

        return base.SetGrantedPermissionsAsync(role, permissions);
    }

    public override async Task<Role> GetRoleByIdAsync(Guid roleId)
    {
        var role = await FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new ApplicationException("There is no role with id: " + roleId);
        }

        return role;
    }

    private void CheckPermissionsToUpdate(Role role, IEnumerable<Permission> permissions)
    {
        if (role.Name == StaticRoleNames.Host.Admin &&
            (!permissions.Any(p => p.Name == PermissionNames.Pages_Administration_Roles_Edit) ||
             !permissions.Any(p => p.Name == PermissionNames.Pages_Administration_Users_ChangePermissions)))
        {
            throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminRole"));
        }
    }

    private new string L(string name)
    {
        return _localizationManager.GetString(DCloudConsts.LocalizationSourceName, name);
    }
}

