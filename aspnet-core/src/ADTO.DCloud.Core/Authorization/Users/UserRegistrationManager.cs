using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Services;
using ADTOSharp.IdentityFramework;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.MultiTenancy;
using ADTOSharp.Linq;
using ADTO.DCloud.Notifications;
using ADTOSharp.Notifications;
using ADTO.DCloud.Configuration;
using ADTOSharp.Configuration;


namespace ADTO.DCloud.Authorization.Users;

public class UserRegistrationManager : DCloudDomainServiceBase
{
    public IADTOSharpSession ADTOSharpSession { get; set; }
    public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IUserEmailer _userEmailer;
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly IAppNotifier _appNotifier;
    private readonly IUserPolicy _userPolicy;


    public UserRegistrationManager(
        TenantManager tenantManager,
        UserManager userManager,
        RoleManager roleManager,
        IPasswordHasher<User> passwordHasher,
        IUserEmailer userEmailer,
        INotificationSubscriptionManager notificationSubscriptionManager,
        IAppNotifier appNotifier,
        IUserPolicy userPolicy)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _passwordHasher = passwordHasher;

        _userEmailer = userEmailer;
        _notificationSubscriptionManager = notificationSubscriptionManager;
        _appNotifier = appNotifier;
        _userPolicy = userPolicy;

        ADTOSharpSession = NullADTOSharpSession.Instance;
        AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
    }

    public async Task<User> RegisterAsync(string name, string emailAddress, string userName, string plainPassword, bool isEmailConfirmed, string emailActivationLink)
    {
        CheckForTenant();
        CheckSelfRegistrationIsEnabled();
        var tenant = await GetActiveTenantAsync();

        var isNewRegisteredUserActiveByDefault = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault);


        var user = new User
        {
            TenantId = tenant.Id,
            Name = name,
            EmailAddress = emailAddress,
            IsActive = isNewRegisteredUserActiveByDefault,
            UserName = userName,
            IsEmailConfirmed = isEmailConfirmed,
            Roles = new List<UserRole>()
        };

        user.SetNormalizedNames();
       
        foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
        {
            user.Roles.Add(new UserRole(tenant.Id, user.Id, defaultRole.Id));
        }

        await _userManager.InitializeOptionsAsync(tenant.Id);

        CheckErrors(await _userManager.CreateAsync(user, plainPassword));
        await CurrentUnitOfWork.SaveChangesAsync();

        if (!user.IsEmailConfirmed)
        {
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(user, emailActivationLink);
        }

        //Notifications
        await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
        await _appNotifier.WelcomeToTheApplicationAsync(user);
        await _appNotifier.NewUserRegisteredAsync(user);


        return user;
    }

    private void CheckForTenant()
    {
        if (!ADTOSharpSession.TenantId.HasValue)
        {
            throw new InvalidOperationException("Can not register host users!");
        }
    }
    private void CheckSelfRegistrationIsEnabled()
    {
      

        if (!SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowSelfRegistration))
        {
            throw new UserFriendlyException(L("SelfUserRegistrationIsDisabledMessage_Detail"));
        }
    }

    private async Task<Tenant> GetActiveTenantAsync()
    {
        if (!ADTOSharpSession.TenantId.HasValue)
        {
            return null;
        }

        return await GetActiveTenantAsync(ADTOSharpSession.TenantId.Value);
    }

    private async Task<Tenant> GetActiveTenantAsync(Guid tenantId)
    {
        var tenant = await _tenantManager.FindByIdAsync(tenantId);
        if (tenant == null)
        {
            throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
        }

        if (!tenant.IsActive)
        {
            throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
        }

        return tenant;
    }

    protected virtual void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }
}

