using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Permissions;
using ADTO.DCloud.Authorization.Permissions.Dto;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Roles.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.IdentityFramework;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Roles;


/// <summary>
/// “角色管理”页面使用的应用程序服务。
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_Roles)]
public class RoleAppService : DCloudAppServiceBase, IRoleAppService
{
    #region Fields
    private readonly RoleManager _roleManager;
    private readonly UserManager _userManager;
    private readonly IRoleManagementConfig _roleManagementConfig;
    private readonly IRepository<UserRole, Guid> _userRoleRepository;
    private readonly IRepository<User, Guid> _userRepository;
    #endregion

    #region Ctor
    public RoleAppService(
    RoleManager roleManager,
    UserManager userManager,
    IRoleManagementConfig roleManagementConfig,
    IRepository<UserRole, Guid> userRoleRepository,
    IRepository<User, Guid> userRepository)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _roleManagementConfig = roleManagementConfig;
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
    }
    #endregion

    #region Utilities

    /// <summary>
    /// 更新角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Roles_Edit)]
    protected virtual async Task UpdateRoleAsync(CreateOrUpdateRoleInput input)
    {
        Debug.Assert(input.Role.Id != null, "input.Role.Id should be set.");

        var role = await _roleManager.GetRoleByIdAsync(input.Role.Id.Value);
        ObjectMapper.Map(input.Role, role);
        await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
    }
    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Roles_Create)]
    protected virtual async Task CreateRoleAsync(CreateOrUpdateRoleInput input)
    {
        var role = ObjectMapper.Map<Role>(input.Role);
        role.TenantId = ADTOSharpSession.TenantId;
        role.SetNormalizedName();
        CheckErrors(await _roleManager.CreateAsync(role));
        await CurrentUnitOfWork.SaveChangesAsync();
        await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
    }

    /// <summary>
    /// 更新角色的权限
    /// </summary>
    /// <param name="role"></param>
    /// <param name="grantedPermissionNames"></param>
    /// <returns></returns>
    private async Task UpdateGrantedPermissionsAsync(Role role, List<string> grantedPermissionNames)
    {
        var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(grantedPermissionNames);
        await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
    }
    #endregion

    #region Methods

    /// <summary>
    /// 角色列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ListResultDto<RoleListDto>> GetRolesAsync(GetRolesInput input)
    {
        var query = _roleManager.Roles;

        if (!input.Keyword.IsNullOrWhiteSpace())
        {
            query = query.Where(w => w.Name.Contains(input.Keyword) || w.DisplayName.Contains(input.Keyword));
        }

        ////获取所有有效的角色
        //var userRoleDtos = await _roleManager.Roles
        //    .OrderBy(r => r.DisplayName)
        //    .Select(r => new UserRoleDto
        //    {
        //        RoleId = r.Id,
        //        RoleName = r.Name,
        //        RoleDisplayName = r.DisplayName
        //    })
        //    .ToArrayAsync();



        if (input.Permissions != null && input.Permissions.Any(p => !string.IsNullOrEmpty(p)))
        {
            input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

            var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                r => r.GrantAllPermissionsByDefault &&
                     r.Side == ADTOSharpSession.MultiTenancySide
            ).Select(r => r.RoleName).ToList();

            foreach (var permission in input.Permissions)
            {
                query = query.Where(r =>
                    r.Permissions.Any(rp => rp.Name == permission)
                        ? r.Permissions.Any(rp => rp.Name == permission && rp.IsGranted)
                        : staticRoleNames.Contains(r.Name)
                );
            }
        }

        var roles = await query.ToListAsync();

        return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(roles));
    }
    /// <summary>
    /// 依ID获取角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<RoleDto> GetRoleByIdAsync(EntityDto<Guid> input)
    {
        var role = await _roleManager.GetRoleByIdAsync(input.Id);
        return ObjectMapper.Map<RoleDto>(role);
    }
    /// <summary>
    /// 获取一个用于编辑时使用的角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Roles_Create, PermissionNames.Pages_Administration_Roles_Edit)]
    public async Task<GetRoleForEditOutput> GetRoleForEditAsync(NullableIdDto<Guid> input)
    {
        var permissions = PermissionManager.GetAllPermissions();
        var grantedPermissions = new Permission[0];
        RoleEditDto roleEditDto;

        if (input.Id.HasValue) //Editing existing role?
        {
            var role = await _roleManager.GetRoleByIdAsync(input.Id.Value);
            grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
            roleEditDto = ObjectMapper.Map<RoleEditDto>(role);
        }
        else
        {
            roleEditDto = new RoleEditDto();
        }

        return new GetRoleForEditOutput
        {
            Role = roleEditDto,
            Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
            GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
        };
    }
    /// <summary>
    /// 新增/编辑角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateRoleAsync(CreateOrUpdateRoleInput input)
    {
        if (input.Role.Id.HasValue)
        {
            await UpdateRoleAsync(input);
        }
        else
        {
            await CreateRoleAsync(input);
        }
    }
    /// <summary>
    /// 依ID删除角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Roles_Delete)]
    public async Task DeleteRoleAsync(EntityDto<Guid> input)
    {
        var role = await _roleManager.GetRoleByIdAsync(input.Id);

        var users = await UserManager.GetUsersInRoleAsync(role.NormalizedName);
        foreach (var user in users)
        {
            CheckErrors(await UserManager.RemoveFromRoleAsync(user, role.NormalizedName));
        }

        CheckErrors(await _roleManager.DeleteAsync(role));
    }



    /// <summary>
    /// 添加用户到角色
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddUsersToRoleAsync(UsersToRoleInput input)
    {
        var role = await _roleManager.GetRoleByIdAsync(input.RoleId);
        foreach (var userId in input.UserIds)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            await UserManager.AddToRoleAsync(user, role.NormalizedName);
            //await UserManager.SetRolesAsync(user, input.AssignedRoleNames)
        }
    }
    #endregion

    #region 角色用户

    /// <summary>
    /// 根据角色Id获取用户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IEnumerable<Guid>> GetUsersByRoleIdAsync(GetUsersByRoleIdInput input)
    {
        var query = from p in _userRoleRepository.GetAll()
                    join f in _userRepository.GetAll() on p.UserId equals f.Id into f_u
                    from user in f_u.DefaultIfEmpty()
                    where p.RoleId.Equals(input.RoleId)
                    select new { p, user };
        var users = await query.ToListAsync();
        var list = users.Select(d => d.user.Id);
        return list;
    }
    /// <summary>
    /// 移除角色里面 的用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task RemoveUserRoleAsync(RemoveUserRoleInput input)
    {
        try
        {
            var user = await _userRepository.FirstOrDefaultAsync(input.UserId);
            if(user==null|| user.Id==Guid.Empty)
                throw new UserFriendlyException("用户不存在");

            //获取所有有效的角色
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();
            foreach (var userRoleDto in userRoleDtos.Where(q=> q.RoleId!=input.RoleId))
            {
                userRoleDto.IsAssigned =  await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
            }
            var roles = userRoleDtos.Where(q => q.IsAssigned == true).Select(d => d.RoleName);
            //重置用户角色
           await UserManager.SetRolesAsync(user, roles.ToArray());

        }
        catch (Exception ex)
        {
            throw new UserFriendlyException("操作失败：" + ex.Message);
        }

    }


    #endregion
}


