using System.Threading.Tasks;
using ADTOSharp.Configuration;
using ADTOSharp.Zero.Configuration;
using ADTO.DCloud.Authorization.Accounts.Dto;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Url;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Authorization.Delegation;
using ADTO.DCloud.Authorization.Impersonation;
using Microsoft.AspNetCore.Identity;
using ADTO.DCloud.Configuration;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Timing;
using ADTOSharp.UI;
using System.Web;
using System;
using ADTOSharp.Extensions;
using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.Runtime.Session;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Security.Recaptcha;

namespace ADTO.DCloud.Authorization.Accounts;

/// <summary>
/// 帐户管理服务
/// </summary>
/// 
public class AccountAppService : DCloudAppServiceBase, IAccountAppService
{
    #region Fields
    // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
    public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";
    public IAppUrlService AppUrlService { get; set; }
    private readonly IUserEmailer _userEmailer;
    private readonly UserRegistrationManager _userRegistrationManager;
    private readonly IImpersonationManager _impersonationManager;
    private readonly IUserLinkManager _userLinkManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IWebUrlService _webUrlService;
    private readonly IUserDelegationManager _userDelegationManager;
    public IRecaptchaValidator RecaptchaValidator { get; set; }
    #endregion

    #region Ctor


    public AccountAppService(
        IUserEmailer userEmailer,
        UserRegistrationManager userRegistrationManager,
        IImpersonationManager impersonationManager,
        IUserLinkManager userLinkManager,
        IPasswordHasher<User> passwordHasher,
        IWebUrlService webUrlService,
        IUserDelegationManager userDelegationManager)
    {
        _userEmailer = userEmailer;
        _userRegistrationManager = userRegistrationManager;
        _impersonationManager = impersonationManager;
        _userLinkManager = userLinkManager;
        _passwordHasher = passwordHasher;
        _webUrlService = webUrlService;

        AppUrlService = NullAppUrlService.Instance;
        RecaptchaValidator = NullRecaptchaValidator.Instance;
        _userDelegationManager = userDelegationManager;
    }
    #endregion

    #region Utilities

    private bool UseCaptchaOnRegistration()
    {
        return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
    }

    private async Task<Tenant> GetActiveTenantAsync(Guid tenantId)
    {
        var tenant = await TenantManager.FindByIdAsync(tenantId);
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

    private async Task<string> GetTenancyNameOrNullAsync(Guid? tenantId)
    {
        return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
    }

    private async Task SendEmailActivationLinkInternal(User user)
    {
        user.SetNewEmailConfirmationCode();
        await _userEmailer.SendEmailActivationLinkAsync(
            user,
            AppUrlService.CreateEmailActivationUrlFormat(ADTOSharpSession.TenantId)
        );
    }
    #endregion

    #region Methods

    /// <summary>
    /// 获取租户状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
    {
        var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
        if (tenant == null)
        {
            return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
        }

        if (!tenant.IsActive)
        {
            return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
        }

        return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id, _webUrlService.GetServerRootAddress(input.TenancyName));
    }
    /// <summary>
    /// 从加密的查询字符串中解析租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Task<Guid?> ResolveTenantId(ResolveTenantIdInput input)
    {
        if (string.IsNullOrEmpty(input.c))
        {
            return Task.FromResult(ADTOSharpSession.TenantId);
        }

        var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
        var query = HttpUtility.ParseQueryString(parameters);

        if (query["tenantId"] == null)
        {
            return Task.FromResult<Guid?>(null);
        }

        var tenantId = Guid.Parse(query["tenantId"]) as Guid?;
        return Task.FromResult(tenantId);
    }
    /// <summary>
    /// 注册新用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<RegisterOutput> Register(RegisterInput input)
    {
        //这里放置验证码功能
        //if (UseCaptchaOnRegistration())
        //{
        //    //await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
        //}

        var user = await _userRegistrationManager.RegisterAsync(
            input.Name,
            input.EmailAddress,
            input.UserName,
            input.Password,
            false,
            AppUrlService.CreateEmailActivationUrlFormat(ADTOSharpSession.TenantId)
        );

        var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

        return new RegisterOutput
        {
            CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
        };
    }

    /// <summary>
    /// 生成密码重置URL+code,发送密码重置邮件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
    {
        var user = await UserManager.FindByEmailAsync(input.EmailAddress);
        if (user == null)
        {
            await Task.Delay(new Random(DateTime.Now.Millisecond).Next(2000, 5000)); // delay a random duration between 2 and 5 seconds to simulate sending an email
            return;
        }

        user.SetNewPasswordResetCode();
        await _userEmailer.SendPasswordResetLinkAsync(
            user,
            AppUrlService.CreatePasswordResetUrlFormat(ADTOSharpSession.TenantId)
        );
    }
    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>

    public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
    {
        if (input.ExpireDate < Clock.Now)
        {
            throw new UserFriendlyException(L("PasswordResetLinkExpired"));
        }

        var user = await UserManager.GetUserByIdAsync(input.UserId);
        if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
        {
            throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
        }

        await UserManager.InitializeOptionsAsync(ADTOSharpSession.TenantId);
        CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
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
    /// <summary>
    /// 发送邮件激活链接
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
    {
        var user = await UserManager.FindByEmailAsync(input.EmailAddress);
        if (user == null)
        {
            return;
        }

        await SendEmailActivationLinkInternal(user);
    }
    /// <summary>
    /// 激活邮件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task ActivateEmail(ActivateEmailInput input)
    {
        var user = await UserManager.FindByIdAsync(input.UserId.ToString());
        if (user != null && user.IsEmailConfirmed)
        {
            return;
        }

        if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
        {
            throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
        }

        user.IsEmailConfirmed = true;
        user.EmailConfirmationCode = null;

        await UserManager.UpdateAsync(user);
    }
    /// <summary>
    /// 修改用户的邮件地址
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ADTOSharpException"></exception>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task ChangeEmail(ChangeEmailInput input)
    {
        var user = await UserManager.FindByIdAsync(input.UserId.ToString());

        if (user == null)
        {
            throw new ADTOSharpException(L("UserNotFound"));
        }

        if (user.EmailAddress != input.OldEmailAddress)
        {
            throw new UserFriendlyException(L("EmailAddressesDidNotMatch"));
        }

        user.EmailAddress = input.EmailAddress;
        user.IsEmailConfirmed = false;

        // May user don't have access new email address. So, we need to reset email confirmation code.
        await SendEmailActivationLinkInternal(user);

        await UserManager.UpdateAsync(user);
    }

    /// <summary>
    /// 模拟登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Users_Impersonation)]
    public virtual async Task<ImpersonateOutput> ImpersonateUser(ImpersonateUserInput input)
    {
        return new ImpersonateOutput
        {
            ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, ADTOSharpSession.TenantId),
            TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
        };
    }
    /// <summary>
    /// 模拟租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ADTOSharpAuthorize(PermissionNames.Pages_Tenants_Impersonation)]
    public virtual async Task<ImpersonateOutput> ImpersonateTenant(ImpersonateTenantInput input)
    {
        return new ImpersonateOutput
        {
            ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, input.TenantId),
            TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
        };
    }
    /// <summary>
    /// 模拟代理
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public virtual async Task<ImpersonateOutput> DelegatedImpersonate(DelegatedImpersonateInput input)
    {
        var userDelegation = await _userDelegationManager.GetAsync(input.UserDelegationId);
        if (userDelegation.TargetUserId != ADTOSharpSession.GetUserId())
        {
            throw new UserFriendlyException("User delegation error.");
        }

        return new ImpersonateOutput
        {
            ImpersonationToken = await _impersonationManager.GetImpersonationToken(userDelegation.SourceUserId, userDelegation.TenantId),
            TenancyName = await GetTenancyNameOrNullAsync(userDelegation.TenantId)
        };
    }
    /// <summary>
    /// 退出模拟帐号
    /// </summary>
    /// <returns></returns>
    public virtual async Task<ImpersonateOutput> BackToImpersonator()
    {
        return new ImpersonateOutput
        {
            ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
            TenancyName = await GetTenancyNameOrNullAsync(ADTOSharpSession.ImpersonatorTenantId)
        };
    }
    /// <summary>
    /// 切换关联帐号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
    {
        if (!await _userLinkManager.AreUsersLinked(ADTOSharpSession.ToUserIdentifier(), input.ToUserIdentifier()))
        {
            throw new Exception(L("This account is not linked to your account"));
        }

        return new SwitchToLinkedAccountOutput
        {
            SwitchAccountToken = await _userLinkManager.GetAccountSwitchToken(input.TargetUserId, input.TargetTenantId),
            TenancyName = await GetTenancyNameOrNullAsync(input.TargetTenantId)
        };
    }


    #endregion
}

