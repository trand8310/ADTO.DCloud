using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using ADTO.DCloud.Authorization.Roles;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Threading;
using ADTOSharp;
using System.Threading.Tasks;
using ADTOSharp.Localization;
using ADTOSharp.UI;
using System.Linq;
using ADTO.DCloud.Security;
using ADTOSharp.Zero.Configuration;
using ADTO.DCloud.Configuration;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Linq;
using System.Threading;
using ADTOSharp.Runtime.Session;
using ADTO.DCloud.Authorization.Posts;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Authorization.Users;

public class UserManager : ADTOSharpUserManager<Role, User>
{

    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ILocalizationManager _localizationManager;
    private readonly ISettingManager _settingManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IRepository<RecentPassword, Guid> _recentPasswords;
    private readonly IRepository<UserPost, Guid> _userpostRepository;
    private readonly IRepository<Post, Guid> _postRepository;

    private readonly IRepository<OrganizationUnit, Guid> _organizationUnitRepository;
    public UserManager(
      RoleManager roleManager,
      UserStore store,
      IOptions<IdentityOptions> optionsAccessor,
      IPasswordHasher<User> passwordHasher,
      IEnumerable<IUserValidator<User>> userValidators,
      IEnumerable<IPasswordValidator<User>> passwordValidators,
      ILookupNormalizer keyNormalizer,
      IdentityErrorDescriber errors,
      IServiceProvider services,
      ILogger<UserManager<User>> logger,
      IPermissionManager permissionManager,
      IUnitOfWorkManager unitOfWorkManager,
      ICacheManager cacheManager,
      IRepository<OrganizationUnit, Guid> organizationUnitRepository,
      IRepository<UserOrganizationUnit, Guid> userOrganizationUnitRepository,
      IOrganizationUnitSettings organizationUnitSettings,
      ISettingManager settingManager,
      ILocalizationManager localizationManager,
      IRepository<RecentPassword, Guid> recentPasswords,
      IRepository<UserLogin, Guid> userLoginRepository,
      IRepository<UserPost, Guid> userpostRepository,
      IRepository<Post, Guid> postRepository)
      : base(
          roleManager,
          store,
          optionsAccessor,
          passwordHasher,
          userValidators,
          passwordValidators,
          keyNormalizer,
          errors,
          services,
          logger,
          permissionManager,
          unitOfWorkManager,
          cacheManager,
          organizationUnitRepository,
          userOrganizationUnitRepository,
          organizationUnitSettings,
          settingManager,
          userLoginRepository)
    {

        _passwordHasher = passwordHasher;
        _unitOfWorkManager = unitOfWorkManager;
        _settingManager = settingManager;
        _localizationManager = localizationManager;
        _recentPasswords = recentPasswords;
        _userpostRepository = userpostRepository;
        _postRepository = postRepository;
    }



    public virtual async Task<User> GetUserOrNullAsync(UserIdentifier userIdentifier)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
            {
                return await FindByIdAsync(userIdentifier.UserId.ToString());
            }
        });
    }

    public User GetUserOrNull(UserIdentifier userIdentifier)
    {
        return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
    }

    public async Task<User> GetUserAsync(UserIdentifier userIdentifier)
    {
        var user = await GetUserOrNullAsync(userIdentifier);
        if (user == null)
        {
            throw new Exception("There is no user: " + userIdentifier);
        }

        return user;
    }

    public User GetUser(UserIdentifier userIdentifier)
    {
        return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
    }

    public override Task<IdentityResult> SetRolesAsync(User user, string[] roleNames)
    {
        if (user.UserName != ADTOSharpUserBase.AdminUserName)
        {
            return base.SetRolesAsync(user, roleNames);
        }

        // Always keep admin role for admin user
        var roles = roleNames.ToList();
        roles.Add(StaticRoleNames.Host.Admin);
        roleNames = roles.ToArray();

        return base.SetRolesAsync(user, roleNames);
    }

    public override async Task SetGrantedPermissionsAsync(User user, IEnumerable<Permission> permissions)
    {
        CheckPermissionsToUpdate(user, permissions);

        await base.SetGrantedPermissionsAsync(user, permissions);
    }

    public async Task<string> CreateRandomPassword()
    {
        var passwordComplexitySetting = new PasswordComplexitySetting
        {
            RequireDigit = await _settingManager.GetSettingValueAsync<bool>(
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit
            ),
            RequireLowercase = await _settingManager.GetSettingValueAsync<bool>(
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase
            ),
            RequireNonAlphanumeric = await _settingManager.GetSettingValueAsync<bool>(
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric
            ),
            RequireUppercase = await _settingManager.GetSettingValueAsync<bool>(
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase
            ),
            RequiredLength = await _settingManager.GetSettingValueAsync<int>(
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength
            )
        };

        var upperCaseLetters = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        var lowerCaseLetters = "abcdefghijkmnopqrstuvwxyz";
        var digits = "0123456789";
        var nonAlphanumerics = "!@$?_-";

        string[] randomChars =
        {
            upperCaseLetters,
            lowerCaseLetters,
            digits,
            nonAlphanumerics
        };

        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();

        if (passwordComplexitySetting.RequireUppercase)
        {
            chars.Insert(rand.Next(0, chars.Count),
                upperCaseLetters[rand.Next(0, upperCaseLetters.Length)]
            );
        }

        if (passwordComplexitySetting.RequireLowercase)
        {
            chars.Insert(rand.Next(0, chars.Count),
                lowerCaseLetters[rand.Next(0, lowerCaseLetters.Length)]
            );
        }

        if (passwordComplexitySetting.RequireDigit)
        {
            chars.Insert(rand.Next(0, chars.Count),
                digits[rand.Next(0, digits.Length)]
            );
        }

        if (passwordComplexitySetting.RequireNonAlphanumeric)
        {
            chars.Insert(rand.Next(0, chars.Count),
                nonAlphanumerics[rand.Next(0, nonAlphanumerics.Length)]
            );
        }

        for (var i = chars.Count; i < passwordComplexitySetting.RequiredLength; i++)
        {
            var rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count),
                rcs[rand.Next(0, rcs.Length)]
            );
        }

        return new string(chars.ToArray());
    }

    private void CheckPermissionsToUpdate(User user, IEnumerable<Permission> permissions)
    {
        if (user.Name == ADTOSharpUserBase.AdminUserName &&
            (!permissions.Any(p => p.Name == PermissionNames.Pages_Administration_Roles_Edit) ||
             !permissions.Any(p => p.Name == PermissionNames.Pages_Administration_Users_ChangePermissions)))
        {
            throw new UserFriendlyException(L("YouCannotRemoveUserRolePermissionsFromAdminUser"));
        }
    }

    private new string L(string name)
    {
        return _localizationManager.GetString(DCloudConsts.LocalizationSourceName, name);
    }

    protected string L(string name, params object[] args) => string.Format(L(name), args);

    public override async Task<IdentityResult> ChangePasswordAsync(User user, string newPassword)
    {
        await CheckRecentPasswordsIfNeeded(user, newPassword);

        var result = await base.ChangePasswordAsync(user, newPassword);

        await StoreRecentPasswordIfNeeded(user, result);

        return result;
    }

    public override async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword,
        string newPassword)
    {
        await CheckRecentPasswordsIfNeeded(user, currentPassword, newPassword);

        var result = await base.ChangePasswordAsync(user, currentPassword, newPassword);

        await StoreRecentPasswordIfNeeded(user, result);

        return result;
    }

    private Task CheckRecentPasswordsIfNeeded(User user, string newPassword)
    {
        return CheckRecentPasswordsIfNeededInternal(user, user.Password, newPassword);
    }

    private Task CheckRecentPasswordsIfNeeded(User user, string currentPassword, string newPassword)
    {
        var currentPasswordHash = _passwordHasher.HashPassword(user, currentPassword);

        return CheckRecentPasswordsIfNeededInternal(user, currentPasswordHash, newPassword);
    }

    private async Task CheckRecentPasswordsIfNeededInternal(
        User user,
        string currentPasswordHash,
        string newPassword)
    {
        var isCheckingLastXPasswordEnabled = await _settingManager.GetSettingValueAsync<bool>(
            AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange
        );

        if (!isCheckingLastXPasswordEnabled)
        {
            return;
        }

        var newPasswordAndCurrentPasswordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            currentPasswordHash,
            newPassword
        );

        var checkingLastXPasswordCount = await _settingManager.GetSettingValueAsync<int>(
            AppSettings.UserManagement.Password
                .CheckingLastXPasswordCount
        );

        if (newPasswordAndCurrentPasswordVerificationResult != PasswordVerificationResult.Failed)
        {
            throw new UserFriendlyException(
                L("NewPasswordMustBeDifferentThenLastXPassword", checkingLastXPasswordCount)
            );
        }

        var recentPasswords = await _recentPasswords.GetAll()
            .Where(rp => rp.UserId == user.Id)
            .OrderByDescending(rp => rp.CreationTime)
            .Take(checkingLastXPasswordCount)
            .ToListAsync();

        foreach (var recentPassword in recentPasswords)
        {
            var recentPasswordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                recentPassword.Password,
                newPassword
            );

            if (recentPasswordVerificationResult != PasswordVerificationResult.Failed)
            {
                throw new UserFriendlyException(
                    L("NewPasswordMustBeDifferentThenLastXPassword", checkingLastXPasswordCount)
                );
            }
        }
    }

    private async Task StoreRecentPasswordIfNeeded(User user, IdentityResult result)
    {
        if (!result.Succeeded)
        {
            return;
        }

        var isCheckingLastXPasswordEnabled = await _settingManager.GetSettingValueAsync<bool>(
            AppSettings.UserManagement.Password.EnableCheckingLastXPasswordWhenPasswordChange
        );

        if (!isCheckingLastXPasswordEnabled)
        {
            return;
        }

        var recentPassword = new RecentPassword
        {
            Password = user.Password,
            UserId = user.Id,
            TenantId = user.TenantId
        };

        await _recentPasswords.InsertAsync(recentPassword);
    }


    /// <summary>
    /// 获取系统中所有管理员用户
    /// </summary>
    /// <returns></returns>
    public async Task<IList<User>> GetUsersInAdminRoleAsync()
    {
        return await ADTOSharpUserStore.GetUsersInRoleAsync(StaticRoleNames.Host.Admin);
    }

    /// <summary>
    /// 获取用户所有的角色
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IList<Role>> GetUserRolesAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await (ADTOSharpUserStore as UserStore).GetUserRolesAsync(user, cancellationToken);
    }
    /// <summary>
    /// 获取当前用户的岗位Ids
    /// </summary>
    /// <returns></returns>
    public async Task<List<Guid>> GetCurrentUserPostIds(Guid? userId = null)
    {
        var _userId = userId.HasValue ? userId : ADTOSharpSession.GetUserId();
        var postIds = await _userpostRepository.GetAll().Where(q => q.UserId.Equals(_userId)).OrderByDescending(q => q.DisplayOrder).Select(f => f.PostId).ToListAsync();
        return postIds;
    }
    /// <summary>
    /// 获取登录者当前岗位-默认按排序字段来，
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> GetCurrentPostId(Guid? userId = null)
    {
        var postList = await GetCurrentUserPostIds(userId);
        // 大于1个岗位才需要设置主主岗所在部门
        if (postList.Count > 0)
        {
            return postList[0];
        }
        return Guid.Empty;
    }

    /// <summary>
    /// 获取登录者当前岗位
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> GetPostId()
    {
        var postList = await GetCurrentUserPostIds();
        if (postList.Count > 1) // 大于1个岗位才需要设置主主岗所在部门
        {
            return postList[0];
        }
        return Guid.Empty;
    }

    /// <summary>
    /// 获取当前部门
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> GetCurrentDepartmentId()
    {
        var postId = await GetPostId();
        if (postId != Guid.Empty)
        {
            var postEntity = await _postRepository.GetAsync(postId);
            return postEntity.DepartmentId;
        }
        var user = await this.GetUserAsync(ADTOSharpSession.ToUserIdentifier());
        return user != null && user.DepartmentId.HasValue ? user.DepartmentId.Value : Guid.Empty;
    }
    /// <summary>
    /// 获取当前部门名称
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetCurrentDepartmentName()
    {
        Guid departMentId =await GetCurrentDepartmentId();
        return (await this._organizationUnitRepository.FirstOrDefaultAsync(p => p.Id == departMentId))?.DisplayName;
    }

    /// <summary>
    /// 获取当前公司
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> GetCurrentCompanyId()
    {
        var postId = await GetPostId();
        if (postId != Guid.Empty)
        {
            var postEntity = await _postRepository.GetAsync(postId);
            return postEntity.CompanyId;
        }
        var user = await this.GetUserAsync(ADTOSharpSession.ToUserIdentifier());
        return user != null && user.DepartmentId.HasValue ? user.DepartmentId.Value : Guid.Empty;
    }

    /// <summary>
    /// 获取当前公司名称
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetCurrentCompanyName()
    {
        Guid companyId = await GetCurrentCompanyId();
        return (await this._organizationUnitRepository.FirstOrDefaultAsync(p => p.Id == companyId))?.DisplayName;
    }
}

