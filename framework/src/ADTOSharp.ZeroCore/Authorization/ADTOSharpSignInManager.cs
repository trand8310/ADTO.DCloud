using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

namespace ADTOSharp.Authorization;

public class ADTOSharpSignInManager<TTenant, TRole, TUser> : SignInManager<TUser>, ITransientDependency
    where TTenant : ADTOSharpTenant<TUser>
    where TRole : ADTOSharpRole<TUser>, new()
    where TUser : ADTOSharpUser<TUser>
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly ISettingManager _settingManager;

    public ADTOSharpSignInManager(
        ADTOSharpUserManager<TRole, TUser> userManager,
        IHttpContextAccessor contextAccessor,
        ADTOSharpUserClaimsPrincipalFactory<TUser, TRole> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<TUser>> logger,
        IUnitOfWorkManager unitOfWorkManager,
        ISettingManager settingManager,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> userConfirmation)
        : base(
            userManager,
            contextAccessor,
            claimsFactory,
            optionsAccessor,
            logger,
            schemes,
            userConfirmation)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _settingManager = settingManager;
    }

    public virtual async Task<SignInResult> SignInOrTwoFactorAsync(ADTOSharpLoginResult<TTenant, TUser> loginResult,
        bool isPersistent, bool? rememberBrowser = null, string loginProvider = null, bool bypassTwoFactor = false)
    {
        if (loginResult.Result != ADTOSharpLoginResultType.Success)
        {
            throw new ArgumentException("loginResult.Result should be success in order to sign in!");
        }

        using (_unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
        {
            await UserManager.As<ADTOSharpUserManager<TRole, TUser>>().InitializeOptionsAsync(loginResult.Tenant?.Id);

            if (!bypassTwoFactor && IsTrue(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled,
                loginResult.Tenant?.Id))
            {
                if (await UserManager.GetTwoFactorEnabledAsync(loginResult.User))
                {
                    if ((await UserManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count > 0)
                    {
                        if (!await IsTwoFactorClientRememberedAsync(loginResult.User) || rememberBrowser == false)
                        {
                            await Context.SignInAsync(
                                IdentityConstants.TwoFactorUserIdScheme,
                                StoreTwoFactorInfo(loginResult.User, loginProvider)
                            );

                            return SignInResult.TwoFactorRequired;
                        }
                    }
                }
            }

            if (loginProvider != null)
            {
                await Context.SignOutAsync(IdentityConstants.ExternalScheme);
            }

            await SignInAsync(loginResult.User, isPersistent, loginProvider);
            return SignInResult.Success;
        }
    }

    public virtual async Task SignOutAndSignInAsync(ClaimsIdentity identity, bool isPersistent)
    {
        await SignOutAsync();
        await SignInAsync(identity, isPersistent);
    }

    public virtual async Task SignInAsync(ClaimsIdentity identity, bool isPersistent)
    {
        await Context.SignInAsync(IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = isPersistent }
        );
    }

    public override async Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            await base.SignInAsync(user, isPersistent, authenticationMethod);
        });
    }

    protected virtual ClaimsPrincipal StoreTwoFactorInfo(TUser user, string loginProvider)
    {
        var identity = new ClaimsIdentity(IdentityConstants.TwoFactorUserIdScheme);

        identity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));

        if (user.TenantId.HasValue)
        {
            identity.AddClaim(new Claim(ADTOSharpClaimTypes.TenantId, user.TenantId.Value.ToString()));
        }

        if (loginProvider != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
        }

        return new ClaimsPrincipal(identity);
    }

    protected virtual async Task<ClaimsPrincipal> StoreRememberClient(TUser user)
    {
        var rememberBrowserIdentity = new ClaimsIdentity(IdentityConstants.TwoFactorRememberMeScheme);

        rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));

        if (user.TenantId.HasValue)
        {
            rememberBrowserIdentity.AddClaim(new Claim(ADTOSharpClaimTypes.TenantId, user.TenantId.Value.ToString()));
        }

        if (UserManager.SupportsUserSecurityStamp)
        {
            var stamp = await UserManager.GetSecurityStampAsync(user);
            rememberBrowserIdentity.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, stamp));
        }

        return new ClaimsPrincipal(rememberBrowserIdentity);
    }

    public async Task<Guid?> GetVerifiedTenantIdAsync()
    {
        var result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);

        if (result?.Principal == null)
        {
            return null;
        }

        return ADTOSharpZeroClaimsIdentityHelper.GetTenantId(result.Principal);
    }

    public override async Task<bool> IsTwoFactorClientRememberedAsync(TUser user)
    {
        var result = await Context.AuthenticateAsync(IdentityConstants.TwoFactorRememberMeScheme);

        return result?.Principal != null &&
               result.Principal.FindFirstValue(ClaimTypes.Name) == user.Id.ToString() &&
               ADTOSharpZeroClaimsIdentityHelper.GetTenantId(result.Principal) == user.TenantId;
    }

    public override async Task RememberTwoFactorClientAsync(TUser user)
    {
        var principal = await StoreRememberClient(user);
        await Context.SignInAsync(IdentityConstants.TwoFactorRememberMeScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }

    private bool IsTrue(string settingName, Guid? tenantId)
    {
        return tenantId == null
            ? _settingManager.GetSettingValueForApplication<bool>(settingName)
            : _settingManager.GetSettingValueForTenant<bool>(settingName, tenantId.Value);
    }
}