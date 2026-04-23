using ADTO.DCloud.Authorization.Accounts.Dto;
using ADTO.DCloud.Authorization.Permissions;
using ADTO.DCloud.Authorization.Permissions.Dto;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Roles.Dto;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Authorization.Users.Exporting;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Exporting;
using ADTO.DCloud.Net.Emailing;
using ADTO.DCloud.Notifications;
using ADTO.DCloud.Organizations.Dto;
using ADTO.DCloud.Url;
using ADTOSharp;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.IdentityFramework;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.Notifications;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADTO.DCloud.Authorization.Users;

/// <summary>
/// “用户管理”页面使用的应用程序服务。
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users)]
public class UserAppService : DCloudAppServiceBase, IUserAppService
{
    #region Fields
    public IAppUrlService AppUrlService { get; set; }
    private readonly RoleManager _roleManager;
    private readonly IUserEmailer _userEmailer;
    private readonly IUserListExcelExporter _userListExcelExporter;
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly IAppNotifier _appNotifier;
    private readonly IRepository<RolePermissionSetting, Guid> _rolePermissionRepository;
    private readonly IRepository<UserPermissionSetting, Guid> _userPermissionRepository;
    private readonly IRepository<UserRole, Guid> _userRoleRepository;
    private readonly IRepository<Role, Guid> _roleRepository;
    private readonly IUserPolicy _userPolicy;
    private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
    private readonly IRoleManagementConfig _roleManagementConfig;
    private readonly UserManager _userManager;
    private readonly IRepository<UserOrganizationUnit, Guid> _userOrganizationUnitRepository;
    private readonly IRepository<OrganizationUnitRole, Guid> _organizationUnitRoleRepository;
    private readonly IOptions<UserOptions> _userOptions;
    private readonly IEmailSettingsChecker _emailSettingsChecker;
    private readonly IPostAppService _postAppService;

    #endregion

    #region Ctor
    public UserAppService(
        RoleManager roleManager,
        IUserEmailer userEmailer,
        IUserListExcelExporter userListExcelExporter,
        INotificationSubscriptionManager notificationSubscriptionManager,
        IAppNotifier appNotifier,
        IRepository<RolePermissionSetting, Guid> rolePermissionRepository,
        IRepository<UserPermissionSetting, Guid> userPermissionRepository,
        IRepository<UserRole, Guid> userRoleRepository,
        IRepository<Role, Guid> roleRepository,
        IUserPolicy userPolicy,
        IEnumerable<IPasswordValidator<User>> passwordValidators,
        IPasswordHasher<User> passwordHasher,
        IRepository<OrganizationUnit, Guid> organizationUnitRepository,
        IRoleManagementConfig roleManagementConfig,
        UserManager userManager,
        IPostAppService postAppService,
        IRepository<UserOrganizationUnit, Guid> userOrganizationUnitRepository,
        IRepository<OrganizationUnitRole, Guid> organizationUnitRoleRepository,
        IOptions<UserOptions> userOptions, IEmailSettingsChecker emailSettingsChecker)
    {
        _roleManager = roleManager;
        _userEmailer = userEmailer;
        _userListExcelExporter = userListExcelExporter;
        _notificationSubscriptionManager = notificationSubscriptionManager;
        _appNotifier = appNotifier;
        _rolePermissionRepository = rolePermissionRepository;
        _userPermissionRepository = userPermissionRepository;
        _userRoleRepository = userRoleRepository;
        _userPolicy = userPolicy;
        _passwordValidators = passwordValidators;
        _passwordHasher = passwordHasher;
        _organizationUnitRepository = organizationUnitRepository;
        _roleManagementConfig = roleManagementConfig;
        _userManager = userManager;
        _userOrganizationUnitRepository = userOrganizationUnitRepository;
        _organizationUnitRoleRepository = organizationUnitRoleRepository;
        _userOptions = userOptions;
        _emailSettingsChecker = emailSettingsChecker;
        _roleRepository = roleRepository;
        _postAppService = postAppService;
        AppUrlService = NullAppUrlService.Instance;
    }
    #endregion

    #region Utilities

    /// <summary>
    /// 设置用户角色信息
    /// </summary>
    /// <param name="userListDtos"></param>
    /// <returns></returns>
    private async Task FillRoleNames(IReadOnlyCollection<UserListDto> userListDtos)
    {
        /* This method is optimized to fill role names to given list. */
        var userIds = userListDtos.Select(u => u.Id);

        var userRoles = await _userRoleRepository.GetAll()
            .Where(userRole => userIds.Contains(userRole.UserId))
            .Select(userRole => userRole).ToListAsync();

        var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

        foreach (var user in userListDtos)
        {
            var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
            user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
        }

        var roleNames = new Dictionary<Guid, string>();
        foreach (var roleId in distinctRoleIds)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role != null)
            {
                roleNames[roleId] = role.DisplayName;
            }
        }

        foreach (var userListDto in userListDtos)
        {
            foreach (var userListRoleDto in userListDto.Roles)
            {
                if (roleNames.ContainsKey(userListRoleDto.RoleId))
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }
            }

            userListDto.Roles = userListDto.Roles.Where(r => r.RoleName != null).OrderBy(r => r.RoleName).ToList();
        }
    }
    /// <summary>
    /// 设置查询用户的过滤条件,为列表或导出接口提供条件过滤
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private IQueryable<User> GetUsersFilteredQuery(IGetUsersInput input)
    {
        var query = UserManager.Users.Include(o => o.Department).Include(c => c.Company)
            .WhereIf(input.Ids != null && input.Ids.Count() > 0, u => input.Ids.Contains(u.Id))
            .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
            .WhereIf(input.OnlyLockedUsers,
                u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
            .WhereIf(
                !input.Filter.IsNullOrWhiteSpace(),
                u =>
                    u.Name.Contains(input.Filter) ||
                    u.UserName.Contains(input.Filter) ||
                    u.EmailAddress.Contains(input.Filter)
            )
            .WhereIf(input.OrganizationUnitId.HasValue, u => u.CompanyId.Equals(input.OrganizationUnitId) || u.DepartmentId.Equals(input.OrganizationUnitId));

        if (input.Permissions != null && input.Permissions.Any(p => !p.IsNullOrWhiteSpace()))
        {
            var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                r => r.GrantAllPermissionsByDefault &&
                     r.Side == ADTOSharpSession.MultiTenancySide
            ).Select(r => r.RoleName).ToList();

            input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

            var userIds = from user in query
                          join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                          from ur in urJoined.DefaultIfEmpty()
                          join urr in _roleRepository.GetAll() on ur.RoleId equals urr.Id into urrJoined
                          from urr in urrJoined.DefaultIfEmpty()
                          join up in _userPermissionRepository.GetAll()
                              .Where(userPermission => input.Permissions.Contains(userPermission.Name)) on user.Id equals up.UserId into upJoined
                          from up in upJoined.DefaultIfEmpty()
                          join rp in _rolePermissionRepository.GetAll()
                              .Where(rolePermission => input.Permissions.Contains(rolePermission.Name)) on
                              new { RoleId = ur == null ? Guid.Empty : ur.RoleId } equals new { rp.RoleId } into rpJoined
                          from rp in rpJoined.DefaultIfEmpty()
                          where (up != null && up.IsGranted) ||
                                (up == null && rp != null && rp.IsGranted) ||
                                (up == null && rp == null && staticRoleNames.Contains(urr.Name))
                          group user by user.Id
                into userGrouped
                          select userGrouped.Key;

            query = UserManager.Users.Where(e => userIds.Contains(e.Id));
        }

        return query;
    }

    /// <summary>
    /// 获取用户所有的角色名称(含直接的角色和所在组织架构中所拥有的角色)
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private List<string> GetAllRoleNamesOfUsersOrganizationUnitsAsync(Guid userId)
    {
        return (from userOu in _userOrganizationUnitRepository.GetAll()
                join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                    .OrganizationUnitId
                join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                where userOu.UserId == userId
                select userOuRoles.Name).ToList();
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ADTOSharpException"></exception>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_Edit)]
    protected virtual async Task<User> UpdateUserAsync(CreateOrUpdateUserInput input)
    {
        Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

        var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());

        if (user is null)
        {
            throw new ADTOSharpException(L("UserNotFound"));
        }

        var isEmailChanged = user.EmailAddress != input.User.EmailAddress;

        if (isEmailChanged)
        {
            user.IsEmailConfirmed = false;
        }

        //Update user properties
        ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)

        CheckErrors(await UserManager.UpdateAsync(user));

        if (input.SetRandomPassword)
        {
            var randomPassword = await _userManager.CreateRandomPassword();
            user.Password = _passwordHasher.HashPassword(user, randomPassword);
            input.User.Password = randomPassword;
        }
        else if (!input.User.Password.IsNullOrEmpty() && !input.User.Password.IsNullOrWhiteSpace())
        {
            await UserManager.InitializeOptionsAsync(ADTOSharpSession.TenantId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
        }

        //更新用户角色
        if (input.AssignedRoleNames != null)
            CheckErrors(await UserManager.SetRolesAsync(user, input.AssignedRoleNames));


        //更新用户OU
        await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

        //更新用户岗位
        if (input.MemberedPosts != null && input.MemberedPosts.Count > 0)
            await _postAppService.SaveUserPostAsync(user.Id, input.MemberedPosts);

        ////更新用户权限
        //if (input.GrantedPermissionNames.Count() > 0)
        //{
        //    var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
        //    await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        //}
        //else
        //{
        //    await UserManager.ResetAllPermissionsAsync(user);
        //}



        if (input.SendActivationEmail || isEmailChanged)
        {
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(ADTOSharpSession.TenantId),
                input.User.Password
            );
        }
        return user;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_Create)]
    protected virtual async Task<User> CreateUserAsync(CreateOrUpdateUserInput input)
    {
        if (ADTOSharpSession.TenantId.HasValue)
        {
            await _userPolicy.CheckMaxUserCountAsync(ADTOSharpSession.GetTenantId());
        }

        var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
        user.TenantId = ADTOSharpSession.TenantId;

        //Set password
        if (input.SetRandomPassword)
        {
            var randomPassword = await _userManager.CreateRandomPassword();
            user.Password = _passwordHasher.HashPassword(user, randomPassword);
            input.User.Password = randomPassword;
        }
        else if (!input.User.Password.IsNullOrEmpty())
        {
            await UserManager.InitializeOptionsAsync(ADTOSharpSession.TenantId);
            foreach (var validator in _passwordValidators)
            {
                CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
            }

            user.Password = _passwordHasher.HashPassword(user, input.User.Password);
        }

        user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

        //分配角色
        user.Roles = new Collection<UserRole>();
        foreach (var roleName in input.AssignedRoleNames)
        {
            var role = await _roleManager.GetRoleByNameAsync(roleName);
            user.Roles.Add(new UserRole(ADTOSharpSession.TenantId, user.Id, role.Id));
        }

        //用户默认部门
        user.DepartmentId = input.OrganizationUnits.Any() ? input.OrganizationUnits.FirstOrDefault() : null;

        CheckErrors(await UserManager.CreateAsync(user));
        await CurrentUnitOfWork.SaveChangesAsync();

        //Notifications
        await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
        await _appNotifier.WelcomeToTheApplicationAsync(user);

        //Organization Units
        await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());


        if (input.MemberedPosts != null && input.MemberedPosts.Count > 0)
            await _postAppService.SaveUserPostAsync(user.Id, input.MemberedPosts);

        ////更新用户权限
        //if (input.GrantedPermissionNames.Count() > 0)
        //{
        //    var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
        //    await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        //}



        //Send activation email
        if (input.SendActivationEmail)
        {
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(ADTOSharpSession.TenantId),
                input.User.Password
            );
        }
        return user;
    }

    #endregion

    #region Methods
    /// <summary>
    /// 用户列表
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PagedResultDto<UserListDto>> GetUsersAsync(GetUsersInput input)
    {
        var query = GetUsersFilteredQuery(input);

        var userCount = await query.CountAsync();

        var users = await query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToListAsync();

        var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
        await FillRoleNames(userListDtos);

        return new PagedResultDto<UserListDto>(
            userCount,
            userListDtos
        );
    }
    public async Task<UserDto> GetUserByIdAsync(EntityDto<Guid> input)
    {
        var user = await UserManager.FindByIdAsync(input.Id.ToString());
        return ObjectMapper.Map<UserDto>(user);
    }

    /// <summary>
    /// 导出用户到EXCEL文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<FileDto> GetUsersToExcelAsync(GetUsersToExcelInput input)
    {
        var query = GetUsersFilteredQuery(input);

        var users = await query
            .OrderBy(input.Sorting)
            .ToListAsync();

        var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
        await FillRoleNames(userListDtos);

        return _userListExcelExporter.ExportToFile(userListDtos, input.SelectedColumns);
    }

    /// <summary>
    /// 导出EXCEL时获取导出数据的列名
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetUserExcelColumnsToExcelAsync()
    {
        return await Task.FromResult(EntityExportHelper.GetEntityColumnNames<UserListDto>());
    }

    /// <summary>
    /// 编辑用户信息时提供该用户的详细信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_Create, PermissionNames.Pages_Administration_Users_Edit)]
    public async Task<GetUserForEditOutput> GetUserForEditAsync(NullableIdDto<Guid> input)
    {
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

        var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

        var output = new GetUserForEditOutput
        {
            Roles = userRoleDtos,
            AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
            MemberedOrganizationUnits = new List<string>(),
            AllowedUserNameCharacters = _userOptions.Value.AllowedUserNameCharacters,
            IsSMTPSettingsProvided = await _emailSettingsChecker.EmailSettingsValidAsync()
        };

        if (!input.Id.HasValue)
        {
            output.User = new UserEditDto
            {
                IsActive = true,
                ShouldChangePasswordOnNextLogin = true,
                IsTwoFactorEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .TwoFactorLogin.IsEnabled),
                IsLockoutEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.UserLockOut
                        .IsEnabled)
            };

            foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
            {
                var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                if (defaultUserRole != null)
                {
                    defaultUserRole.IsAssigned = true;
                }
            }
            output.MemberedPosts = new List<Posts.Dto.PostDto>();
        }
        else
        {
            var user = await UserManager.GetUserByIdAsync(input.Id.Value);

            output.User = ObjectMapper.Map<UserEditDto>(user);
            output.ProfilePictureId = user.ProfilePictureId;
            if (user.ManagerId.HasValue)
            {
                var managerInfo = await UserManager.GetUserByIdAsync(user.ManagerId.Value);
                output.User.ManagerName = managerInfo != null ? managerInfo.Name : "";
            }


            var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
            output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();
            output.MemberedPosts = await _postAppService.GetPostByUser(new Posts.Dto.GetPostByUserInput() { UserId = user.Id });






            var allRolesOfUsersOrganizationUnits = GetAllRoleNamesOfUsersOrganizationUnitsAsync(input.Id.Value);

            foreach (var userRoleDto in userRoleDtos)
            {
                userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                userRoleDto.InheritedFromOrganizationUnit =
                    allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
            }
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);
            output.GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList();
        }

        return output;
    }

    /// <summary>
    /// 获取用户所拥有的权限,用于修改用户当前所拥有的所有权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
    public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEditAsync(EntityDto<Guid> input)
    {
        var user = await UserManager.GetUserByIdAsync(input.Id);
        var permissions = PermissionManager.GetAllPermissions();
        var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

        return new GetUserPermissionsForEditOutput
        {
            Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName)
                .ToList(),
            GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
        };


    }

    /// <summary>
    /// 重设用户权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
    public async Task ResetUserSpecificPermissionsAsync(EntityDto<Guid> input)
    {
        var user = await UserManager.GetUserByIdAsync(input.Id);
        await UserManager.ResetAllPermissionsAsync(user);
    }
    /// <summary>
    /// 更新用户权限,与 "GetUserPermissionsForEditAsync" 结合使用
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_ChangePermissions)]
    public async Task UpdateUserPermissionsAsync(UpdateUserPermissionsInput input)
    {
        var user = await UserManager.GetUserByIdAsync(input.Id);
        var grantedPermissions =
            PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
        await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
    }
    /// <summary>
    /// 创建/更新用户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<User> CreateOrUpdateUserAsync(CreateOrUpdateUserInput input)
    {
        if (input.User.Id.HasValue)
        {
            return await UpdateUserAsync(input);
        }
        else
        {
            return await CreateUserAsync(input);
        }
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_Delete)]
    public async Task DeleteUserAsync(EntityDto<Guid> input)
    {
        if (input.Id == ADTOSharpSession.GetUserId())
        {
            throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
        }

        var user = await UserManager.GetUserByIdAsync(input.Id);
        CheckErrors(await UserManager.DeleteAsync(user));
    }

    /// <summary>
    /// 查询用户信息-不分页，不需要权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [ADTOSharpAllowAnonymous]
    public async Task<List<UserSimpleDto>> GetUserSimpleDtoList()
    {
        if (!ADTOSharpSession.UserId.HasValue)
        {
            throw new UserFriendlyException("请登录.");
        }
        var query = from user in this.UserManager.Users

                    join d in _organizationUnitRepository.GetAll() on user.DepartmentId equals d.Id into deptment
                    from deptmentInfo in deptment.DefaultIfEmpty()
                    where user.IsActive && user.UserName != "admin" && user.UserName != "guest"
                    select new UserSimpleDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        UserName = user.UserName,
                        Department = deptmentInfo != null ? deptmentInfo.DisplayName : "",
                        DepartmentId = user.DepartmentId,
                        CompanyId = user.CompanyId
                    };
        //获取总数
        var resultCount = await query.CountAsync();
        return query.ToList();
    }



    /// <summary>
    /// 启用用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task Activate(EntityDto<Guid> input)
    {
        var user = await UserManager.GetUserByIdAsync(input.Id);
        user.IsActive = true;
        await UserManager.UpdateAsync(user);
    }
    /// <summary>
    /// 禁用用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task DeActivate(EntityDto<Guid> input)
    {
        var user = await UserManager.GetUserByIdAsync(input.Id);
        user.IsActive = false;
        await UserManager.UpdateAsync(user);
    }
    /// <summary>
    /// 用户锁定-解锁
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task Unlock(EntityDto<Guid> input)
    {
        var user = await UserManager.GetUserByIdAsync(input.Id);
        user.IsLockoutEnabled = false;
        user.LockoutEndDateUtc = null;
        await UserManager.UpdateAsync(user);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordDto input)
    {
        if (!ADTOSharpSession.UserId.HasValue)
        {
            throw new UserFriendlyException(L("PleaseLogInBeforeAttemptingToResetPassword"));
        }
        var user = await UserManager.GetUserByIdAsync(input.UserId);
        if (user == null)
        {
            throw new UserFriendlyException(L("User.Null"));
        }
        await UserManager.InitializeOptionsAsync(ADTOSharpSession.TenantId);
        CheckErrors(await UserManager.ChangePasswordAsync(user, input.NewPassword));
        user.PasswordResetCode = null;
        user.IsEmailConfirmed = true;
        user.ShouldChangePasswordOnNextLogin = false;
        await UserManager.UpdateAsync(user);
        return new ResetPasswordOutput
        {
            CanLogin = user.IsActive,
            UserName = user.UserName
        };
    }
    #endregion
}
