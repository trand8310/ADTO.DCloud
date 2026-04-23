using Microsoft.AspNetCore.Identity;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Zero.Configuration;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.MultiTenancy;
using System;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization;

public class LogInManager : ADTOSharpLogInManager<Tenant, Role, User>
{
    public LogInManager(
        UserManager userManager,
        IMultiTenancyConfig multiTenancyConfig,
        IRepository<Tenant, Guid> tenantRepository,
        IUnitOfWorkManager unitOfWorkManager,
        ISettingManager settingManager,
        IRepository<UserLoginAttempt, Guid> userLoginAttemptRepository,
        IUserManagementConfig userManagementConfig,
        IIocResolver iocResolver,
        IPasswordHasher<User> passwordHasher,
        RoleManager roleManager,
        UserClaimsPrincipalFactory claimsPrincipalFactory)
        : base(
              userManager,
              multiTenancyConfig,
              tenantRepository,
              unitOfWorkManager,
              settingManager,
              userLoginAttemptRepository,
              userManagementConfig,
              iocResolver,
              passwordHasher,
              roleManager,
              claimsPrincipalFactory)
    {
    }


    /// <summary>
    /// Exposes protected method CreateLoginResultAsync
    /// </summary>
    /// <param name="user">User to create login result</param>
    /// <param name="tenant">Tenant of the given user</param>
    /// <returns></returns>
    public new Task<ADTOSharpLoginResult<Tenant, User>> CreateLoginResultAsync(User user, Tenant tenant = null)
    {
        return base.CreateLoginResultAsync(user, tenant);
    }
}
