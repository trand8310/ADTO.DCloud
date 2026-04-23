using ADTO.DCloud.Authentication;
using ADTO.DCloud.Authentication.External;
using ADTO.DCloud.Authentication.JwtBearer;
using ADTO.DCloud.Authentication.PasswordlessLogin;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Accounts.Dto;
using ADTO.DCloud.Authorization.Delegation;
using ADTO.DCloud.Authorization.Impersonation;
using ADTO.DCloud.Authorization.PasswordlessLogin;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.TwoFactor;
using ADTO.DCloud.Authorization.TwoFactor.Google;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.Identity;
using ADTO.DCloud.Models.TokenAuth;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Net.Sms;
using ADTO.DCloud.Notifications;
using ADTO.DCloud.Security.Recaptcha;
using ADTO.DCloud.Web.Authentication.External;
using ADTO.DCloud.Web.Authentication.JwtBearer;
using ADTO.DCloud.Web.Authentication.TwoFactor;
using ADTO.DCloud.Web.Common;
using ADTO.DCloud.Web.Models.TokenAuth;
using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Net.Mail;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing;
using ADTOSharp.UI;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;


namespace ADTO.DCloud.Controllers;

/// <summary>
/// 授权管理
/// </summary>
[Route("api/[controller]/[action]")]
public class TokenAuthController : DCloudControllerBase
{
    #region Fields
    private readonly LogInManager _logInManager;
    private readonly ITenantCache _tenantCache;
    private readonly ICacheManager _cacheManager;
    private readonly UserManager _userManager;
    private readonly ADTOSharpLoginResultTypeHelper _loginResultTypeHelper;

    private readonly TokenAuthConfiguration _configuration;
    private readonly IdentityOptions _identityOptions;
    private readonly IOptions<AsyncJwtBearerOptions> _jwtOptions;
    private readonly IJwtSecurityStampHandler _securityStampHandler;
    private readonly ADTOSharpUserClaimsPrincipalFactory<User, Role> _claimsPrincipalFactory;
    private readonly IUserLinkManager _userLinkManager;
    private readonly IAppNotifier _appNotifier;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly IImpersonationManager _impersonationManager;
    private readonly IUserDelegationManager _userDelegationManager;
    private readonly GoogleAuthenticatorProvider _googleAuthenticatorProvider;
    private readonly IExternalAuthConfiguration _externalAuthConfiguration;
    private readonly IExternalAuthManager _externalAuthManager;
    private readonly ISettingManager _settingManager;
    private readonly IPasswordlessLoginManager _passwordlessLoginManager;
    private readonly ExternalLoginInfoManagerFactory _externalLoginInfoManagerFactory;
    private readonly UserRegistrationManager _userRegistrationManager;


    public IRecaptchaValidator RecaptchaValidator { get; set; }
    #endregion

    #region Ctor
    public TokenAuthController(
        LogInManager logInManager,
        ITenantCache tenantCache,
        ICacheManager cacheManager,
        UserManager userManager,
        IImpersonationManager impersonationManager,
        IUserLinkManager userLinkManager,
        IAppNotifier appNotifier,
        ISmsSender smsSender,
        IEmailSender emailSender,
        IExternalAuthConfiguration externalAuthConfiguration,
        IExternalAuthManager externalAuthManager,
        GoogleAuthenticatorProvider googleAuthenticatorProvider,
        IJwtSecurityStampHandler securityStampHandler,
        ADTOSharpLoginResultTypeHelper loginResultTypeHelper,
        ADTOSharpUserClaimsPrincipalFactory<User, Role> claimsPrincipalFactory,
        TokenAuthConfiguration configuration,
        IOptions<AsyncJwtBearerOptions> jwtOptions,
        IOptions<IdentityOptions> identityOptions,
        IUserDelegationManager userDelegationManager,
        IPasswordlessLoginManager passwordlessLoginManager,
        UserRegistrationManager userRegistrationManager,
        ExternalLoginInfoManagerFactory externalLoginInfoManagerFactory,
        ISettingManager settingManager
        )
    {
        _logInManager = logInManager;
        _tenantCache = tenantCache;
        _cacheManager = cacheManager;
        _userManager = userManager;
        _impersonationManager = impersonationManager;
        _userLinkManager = userLinkManager;
        _appNotifier = appNotifier;
        _smsSender = smsSender;
        _emailSender = emailSender;
        _externalAuthConfiguration = externalAuthConfiguration;
        _externalAuthManager = externalAuthManager;
        _googleAuthenticatorProvider = googleAuthenticatorProvider;
        _securityStampHandler = securityStampHandler;
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _loginResultTypeHelper = loginResultTypeHelper;
        _configuration = configuration;
        _jwtOptions = jwtOptions;
        _identityOptions = identityOptions.Value;
        _userDelegationManager = userDelegationManager;
        _passwordlessLoginManager = passwordlessLoginManager;
        _settingManager = settingManager;
        _userRegistrationManager = userRegistrationManager;
        _externalLoginInfoManagerFactory = externalLoginInfoManagerFactory;
        RecaptchaValidator = NullRecaptchaValidator.Instance;

    }
    #endregion

    #region Utilities

    private string GetTenancyNameOrNull()
    {
        if (!ADTOSharpSession.TenantId.HasValue)
        {
            return null;
        }

        return _tenantCache.GetOrNull(ADTOSharpSession.TenantId.Value)?.TenancyName;
    }

    /// <summary>
    /// 移除令牌
    /// </summary>
    /// <param name="tokenKey"></param>
    /// <returns></returns>
    private async Task RemoveTokenAsync(string tokenKey)
    {
        await _userManager.RemoveTokenValidityKeyAsync(
            await _userManager.GetUserAsync(ADTOSharpSession.ToUserIdentifier()), tokenKey
        );

        await _cacheManager.GetCache(AppConsts.TokenValidityKey).RemoveAsync(tokenKey);
    }

    private async Task<ADTOSharpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
    {
        var shouldLockout = await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.UserLockOut.IsEnabled);
        var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName, shouldLockout);

        switch (loginResult.Result)
        {
            case ADTOSharpLoginResultType.Success:
                return loginResult;
            default:
                throw _loginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                    loginResult.Result,
                    usernameOrEmailAddress,
                    tenancyName
                );
        }
    }
    /// <summary>
    /// 生成重置密码的URL,并加密查询字符串部份.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="tenant"></param>
    /// <param name="passwordResetCode"></param>
    /// <returns></returns>
    private async Task<string> EncryptQueryParameters(Guid userId, Tenant tenant, string passwordResetCode)
    {
        var expirationHours = await _settingManager.GetSettingValueAsync<int>(
            AppSettings.UserManagement.Password.PasswordResetCodeExpirationHours
        );

        var expireDate = Uri.EscapeDataString(Clock.Now.AddHours(expirationHours)
            .ToString(DCloudConsts.DateTimeOffsetFormat));

        var query = $"userId={userId}&resetCode={passwordResetCode}&expireDate={expireDate}";

        if (tenant != null)
        {
            query += $"&tenantId={tenant.Id}";
        }

        return SimpleStringCipher.Instance.Encrypt(query);
    }

    /// <summary>
    /// 判断登录是否使用了图形验证码
    /// </summary>
    /// <returns></returns>
    private bool UseCaptchaOnLogin()
    {
        return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnLogin);
    }
    private async Task ValidateReCaptcha(string captchaResponse)
    {
        var requestUserAgent = Request.Headers["User-Agent"].ToString();

        if (requestUserAgent.IsNullOrEmpty())
        {
            return;
        }

        if (WebConsts.ReCaptchaIgnoreWhiteList.Contains(requestUserAgent.Trim()))
        {
            return;
        }

        await RecaptchaValidator.ValidateAsync(captchaResponse);
    }


    private bool IsSchemeEnabledOnTenant(ExternalLoginProviderInfo scheme)
    {
        if (!ADTOSharpSession.TenantId.HasValue)
        {
            return true;
        }

        switch (scheme.Name)
        {
            case "OpenIdConnect":
                return !_settingManager.GetSettingValueForTenant<bool>(
                    AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsDeactivated, ADTOSharpSession.GetTenantId());
            case "Microsoft":
                return !_settingManager.GetSettingValueForTenant<bool>(
                    AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsDeactivated, ADTOSharpSession.GetTenantId());
            case "Google":
                return !_settingManager.GetSettingValueForTenant<bool>(
                    AppSettings.ExternalLoginProvider.Tenant.Google_IsDeactivated, ADTOSharpSession.GetTenantId());
            case "Twitter":
                return !_settingManager.GetSettingValueForTenant<bool>(
                    AppSettings.ExternalLoginProvider.Tenant.Twitter_IsDeactivated, ADTOSharpSession.GetTenantId());
            case "Facebook":
                return !_settingManager.GetSettingValueForTenant<bool>(
                    AppSettings.ExternalLoginProvider.Tenant.Facebook_IsDeactivated, ADTOSharpSession.GetTenantId());
            case "WsFederation":
                return !_settingManager.GetSettingValueForTenant<bool>(
                    AppSettings.ExternalLoginProvider.Tenant.WsFederation_IsDeactivated, ADTOSharpSession.GetTenantId());
            default: return true;
        }
    }



    private async Task<ADTOSharpLoginResult<Tenant, User>> GetPasswordlessLoginResultAsync(User user)
    {
        var loginResult = await _logInManager.CreateLoginResultAsync(user);

        switch (loginResult.Result)
        {
            case ADTOSharpLoginResultType.Success:
                return loginResult;
            default:
                throw _loginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                    loginResult.Result,
                    user.EmailAddress,
                    GetTenancyNameOrNull()
                );
        }
    }


    private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
    {
        return CreateToken(claims, expiration ?? _configuration.AccessTokenExpiration);
    }
    private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, User user, TimeSpan? expiration = null, TokenType tokenType = TokenType.AccessToken, string refreshTokenKey = null)
    {
        var tokenValidityKey = Guid.NewGuid().ToString();
        var claims = identity.Claims.ToList();
        var nameIdClaim = claims.First(c => c.Type == _identityOptions.ClaimsIdentity.UserIdClaimType);

        if (_identityOptions.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
        }

        claims.AddRange(new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64),
            new Claim(AppConsts.TokenValidityKey, tokenValidityKey),
            new Claim(AppConsts.UserIdentifier, user.ToUserIdentifier().ToUserIdentifierString()),
            new Claim(AppConsts.TokenType, tokenType.To<int>().ToString())
        });

        if (!string.IsNullOrEmpty(refreshTokenKey))
        {
            claims.Add(new Claim(AppConsts.RefreshTokenValidityKey, refreshTokenKey));
        }

        if (!expiration.HasValue)
        {
            expiration = tokenType == TokenType.AccessToken
                ? _configuration.AccessTokenExpiration
                : _configuration.RefreshTokenExpiration;
        }

        var expirationDate = DateTime.UtcNow.Add(expiration.Value);

        await _cacheManager
            .GetCache(AppConsts.TokenValidityKey)
            .SetAsync(tokenValidityKey, "", absoluteExpireTime: new DateTimeOffset(expirationDate));

        await _userManager.AddTokenValidityKeyAsync(
            user,
            tokenValidityKey,
            expirationDate
        );

        return claims;
    }
    private string GetEncryptedAccessToken(string accessToken)
    {
        return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
    }
    private (string token, string key) CreateRefreshToken(IEnumerable<Claim> claims)
    {
        var claimsList = claims.ToList();
        return (CreateToken(claimsList, AppConsts.RefreshTokenExpiration),
            claimsList.First(c => c.Type == AppConsts.TokenValidityKey).Value);
    }
    private string CreateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
    {

        var now = Clock.Now;// DateTime.UtcNow;

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            claims: claims,
            notBefore: now,
            signingCredentials: _configuration.SigningCredentials,
            //expires: now.Add(expiration ?? _configuration.AccessTokenExpiration)
            expires: expiration == null ? (DateTime?)null : now.Add(expiration.Value)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
    /// <summary>
    /// 清除指定用户的所有其它登录凭据
    /// </summary>
    /// <param name="loginResult"></param>
    /// <returns></returns>
    private async Task ResetSecurityStampForLoginResult(ADTOSharpLoginResult<Tenant, User> loginResult)
    {
        await _userManager.UpdateSecurityStampAsync(loginResult.User);
        await _securityStampHandler.SetSecurityStampCacheItem(loginResult.User.TenantId, loginResult.User.Id,
            loginResult.User.SecurityStamp);
        loginResult.Identity.ReplaceClaim(new Claim(AppConsts.SecurityStampKey, loginResult.User.SecurityStamp));
    }
    /// <summary>
    /// 获取用户设置"单一登录"状态值,为true时,同一个帐号,在系统运行期间只能存在唯一的登录记录
    /// </summary>
    /// <returns></returns>
    private bool AllowOneConcurrentLoginPerUser()
    {
        return _settingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser);
    }
    private async Task<(bool isValid, ClaimsPrincipal principal)> IsRefreshTokenValid(string refreshToken)
    {
        ClaimsPrincipal principal = null;

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = _configuration.Audience,
                ValidIssuer = _configuration.Issuer,
                IssuerSigningKey = _configuration.SecurityKey
            };

            foreach (var validator in _jwtOptions.Value.AsyncSecurityTokenValidators)
            {
                if (!validator.CanReadToken(refreshToken))
                {
                    continue;
                }

                try
                {
                    (principal, _) = await validator.ValidateRefreshToken(refreshToken, validationParameters);
                    return (true, principal);
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex.ToString(), ex);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Debug(ex.ToString(), ex);
        }

        return (false, principal);
    }


    private static string AddSingleSignInParametersToReturnUrl(string returnUrl, string signInToken, Guid userId, Guid? tenantId)
    {
        returnUrl += (returnUrl.Contains("?") ? "&" : "?") +
                     "accessToken=" + signInToken +
                     "&userId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
        if (tenantId.HasValue)
        {
            returnUrl += "&tenantId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(tenantId.Value.ToString()));
        }

        return returnUrl;
    }

    private async Task<bool> IsTwoFactorAuthRequiredAsync(ADTOSharpLoginResult<Tenant, User> loginResult, AuthenticateModel authenticateModel)
    {
        if (!await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled))
        {
            return false;
        }

        if (!loginResult.User.IsTwoFactorEnabled)
        {
            return false;
        }

        if ((await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count <= 0)
        {
            return false;
        }

        if (await TwoFactorClientRememberedAsync(loginResult.User.ToUserIdentifier(), authenticateModel))
        {
            return false;
        }

        return true;
    }
    private async Task<bool> TwoFactorClientRememberedAsync(UserIdentifier userIdentifier, AuthenticateModel authenticateModel)
    {
        if (!await SettingManager.GetSettingValueAsync<bool>(
                ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled)
           )
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(authenticateModel.TwoFactorRememberClientToken))
        {
            return false;
        }

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = _configuration.Audience,
                ValidIssuer = _configuration.Issuer,
                IssuerSigningKey = _configuration.SecurityKey
            };

            foreach (var validator in _jwtOptions.Value.AsyncSecurityTokenValidators)
            {
                if (validator.CanReadToken(authenticateModel.TwoFactorRememberClientToken))
                {
                    try
                    {
                        var (principal, _) = await validator.ValidateToken(
                            authenticateModel.TwoFactorRememberClientToken,
                            validationParameters
                        );

                        var userIdentifierClaim = principal.FindFirst(c => c.Type == AppConsts.UserIdentifier);
                        if (userIdentifierClaim == null)
                        {
                            return false;
                        }

                        return userIdentifierClaim.Value == userIdentifier.ToString();
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.ToString(), ex);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Debug(ex.ToString(), ex);
        }

        return false;
    }

    /* 检查两个因素代码并返回一个令牌，以便在需要时浏览器记住客户端 */
    private async Task<string> TwoFactorAuthenticateAsync(ADTOSharpLoginResult<Tenant, User> loginResult, AuthenticateModel authenticateModel)
    {
        var twoFactorCodeCache = _cacheManager.GetTwoFactorCodeCache();
        var userIdentifier = loginResult.User.ToUserIdentifier().ToString();
        var cachedCode = await twoFactorCodeCache.GetOrDefaultAsync(userIdentifier);
        var provider = _cacheManager.GetCache("ProviderCache").Get("Provider", cache => cache).ToString();

        if (provider == GoogleAuthenticatorProvider.Name)
        {
            if (!await _googleAuthenticatorProvider.ValidateAsync("TwoFactor",
                    authenticateModel.TwoFactorVerificationCode, _userManager, loginResult.User))
            {
                throw new UserFriendlyException(L("InvalidSecurityCode"));
            }
        }
        else if (cachedCode?.Code == null || cachedCode.Code != authenticateModel.TwoFactorVerificationCode)
        {
            throw new UserFriendlyException(L("InvalidSecurityCode"));
        }

        //Delete from the cache since it was a single usage code
        await twoFactorCodeCache.RemoveAsync(userIdentifier);

        if (authenticateModel.RememberClient)
        {
            if (await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin
                    .IsRememberBrowserEnabled))
            {
                return CreateAccessToken(
                    await CreateJwtClaims(
                        loginResult.Identity,
                        loginResult.User
                    )
                );
            }
        }

        return null;
    }
    private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
    {
        var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
        if (model.ProviderKey.IsNullOrWhiteSpace())
            model.ProviderKey = userInfo.ProviderKey;

        if (!ProviderKeysAreEqual(model, userInfo))
        {
            throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
        }

        return userInfo;
    }
    private bool ProviderKeysAreEqual(ExternalAuthenticateModel model, ExternalAuthUserInfo userInfo)
    {
        if (userInfo.ProviderKey == model.ProviderKey)
        {
            return true;
        }
        return userInfo.ProviderKey == model.ProviderKey.Replace("-", "").TrimStart('0');

    }

    #endregion


    #region Methods

    /// <summary>
    /// 登录授权
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
    {
        if (UseCaptchaOnLogin())
        {
            await ValidateReCaptcha(model.CaptchaResponse);
        }

        var loginResult = await GetLoginResultAsync(
            model.UserName,
            model.Password,
            GetTenancyNameOrNull()
        );

        var returnUrl = model.ReturnUrl;

        //if (model.SingleSignIn.HasValue && model.SingleSignIn.Value &&
        //    loginResult.Result == ADTOSharpLoginResultType.Success)
        //{
        //    loginResult.User.SetSignInToken();
        //    returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken,
        //        loginResult.User.Id, loginResult.User.TenantId);
        //}

        //校验"用户下次登录使用时,必须更改密码"
        if (loginResult.User.ShouldChangePasswordOnNextLogin)
        {
            loginResult.User.SetNewPasswordResetCode();
            return new AuthenticateResultModel
            {
                ShouldResetPassword = true,
                ReturnUrl = returnUrl,
                c = await EncryptQueryParameters(loginResult.User.Id, loginResult.Tenant, loginResult.User.PasswordResetCode)
            };
        }


        //Two factor auth
        await _userManager.InitializeOptionsAsync(loginResult.Tenant?.Id);

        string twoFactorRememberClientToken = null;
        if (await IsTwoFactorAuthRequiredAsync(loginResult, model))
        {
            if (model.TwoFactorVerificationCode.IsNullOrEmpty())
            {
                //Add a cache item which will be checked in SendTwoFactorAuthCode to prevent sending unwanted two factor code to users.
                await _cacheManager
                    .GetTwoFactorCodeCache()
                    .SetAsync(
                        loginResult.User.ToUserIdentifier().ToString(),
                        new TwoFactorCodeCacheItem()
                    );

                return new AuthenticateResultModel
                {
                    RequiresTwoFactorVerification = true,
                    UserId = loginResult.User.Id,
                    TwoFactorAuthProviders = await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User),
                    ReturnUrl = returnUrl
                };
            }

            twoFactorRememberClientToken = await TwoFactorAuthenticateAsync(loginResult, model);
        }

        // One Concurrent Login 
        //if (AllowOneConcurrentLoginPerUser())
        //{
        //    await ResetSecurityStampForLoginResult(loginResult);
        //}

        var refreshToken = CreateRefreshToken(
            await CreateJwtClaims(
                loginResult.Identity,
                loginResult.User,
                tokenType: TokenType.RefreshToken
            )
        );

        var accessToken = CreateAccessToken(
            await CreateJwtClaims(
                loginResult.Identity,
                loginResult.User,
                refreshTokenKey: refreshToken.key
            )
        );



        // var enc = SimpleStringCipher.Instance.Encrypt(accessToken, DCloudConsts.DefaultPassPhrase);// GetEncryptedAccessToken(accessToken);

        //var dec = SimpleStringCipher.Instance.Decrypt(enc, DCloudConsts.DefaultPassPhrase);

        return new AuthenticateResultModel
        {
            AccessToken = accessToken,
            ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds,
            RefreshToken = refreshToken.token,
            RefreshTokenExpireInSeconds = (int)_configuration.RefreshTokenExpiration.TotalSeconds,
            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
            TwoFactorRememberClientToken = twoFactorRememberClientToken,
            UserId = loginResult.User.Id,
            ReturnUrl = returnUrl
        };

    }

    //[HttpPost]
    //public async Task SendPasswordlessLoginCode(SendPasswordlessLoginCodeInput input)
    //{
    //    if (input.ProviderType == PasswordlessLoginProviderType.Email)
    //    {
    //        //await SendEmailPasswordlessCode(input.ProviderValue);
    //    }
    //    else if (input.ProviderType == PasswordlessLoginProviderType.Sms)
    //    {
    //        // await SendSmsPasswordlessCode(input.ProviderValue);
    //    }
    //    else if (input.ProviderType == PasswordlessLoginProviderType.QRCODE)
    //    {
    //        //await SendQRCodePasswordlessCode(input.ProviderValue);
    //        await SendQRCodePasswordlessCode(input.ProviderValue);
    //    }
    //}


    /// <summary>
    /// 生成PasswordlessCode
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<string> GeneratePasswordlessLoginCode([FromQuery] string id)
    {
        if (id.IsNullOrEmpty())
        {
            return Guid.Empty.ToString("N");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return Guid.Empty.ToString("N");
        }

        var code = await _passwordlessLoginManager.GeneratePasswordlessLoginCode(
            ADTOSharpSession.TenantId,
            id
        );
        return code;
    }




    /// <summary>
    /// 无密码登录,依据系统给的凭据生成TOKEN,支持QRCODE,EMAIL,SMS,微信小程序...
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PasswordlessAuthenticateResultModel> PasswordlessAuthenticate([FromBody] PasswordlessAuthenticateModel model)
    {
        if (UseCaptchaOnLogin())
        {
            await ValidateReCaptcha(model.CaptchaResponse);
        }

        if (model.VerificationCode.IsNullOrEmpty())
        {
            return new PasswordlessAuthenticateResultModel();
        }

        await _passwordlessLoginManager.VerifyPasswordlessLoginCode(
            ADTOSharpSession.TenantId,
            model.ProviderValue,
            model.VerificationCode
        );

        var user = await _passwordlessLoginManager.GetUserByPasswordlessProviderAndKeyAsync(
            model.Provider,
            model.ProviderValue
        );

        var loginResult = await GetPasswordlessLoginResultAsync(user);

        //var returnUrl = model.ReturnUrl ?? "";

        //if (model.SingleSignIn.HasValue && model.SingleSignIn.Value &&
        //    loginResult.Result == ADTOSharpLoginResultType.Success)
        //{
        //    loginResult.User.SetSignInToken();
        //    returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl ?? "", loginResult.User.SignInToken,
        //        loginResult.User.Id, loginResult.User.TenantId);
        //}

        //var refreshToken = CreateRefreshToken(
        //await CreateJwtClaims(
        //loginResult.Identity,
        //        loginResult.User,
        //        tokenType: TokenType.RefreshToken
        //    )
        //);

        var accessToken = CreateAccessToken(
            await CreateJwtClaims(
                loginResult.Identity,
                loginResult.User
            //refreshTokenKey: refreshToken.key
            )
        );

        //await _passwordlessLoginManager.RemovePasswordlessLoginCode(
        //    ADTOSharpSession.TenantId,
        //    model.ProviderValue
        //);

        return new PasswordlessAuthenticateResultModel
        {
            AccessToken = accessToken,
            ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds,
            //RefreshToken = refreshToken.token,
            //RefreshTokenExpireInSeconds = (int)_configuration.RefreshTokenExpiration.TotalSeconds,
            //EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
            UserId = loginResult.User.Id,
        };
    }



    /// <summary>
    /// 登录授权,这个方法,主要用于测试,后期废弃
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<LoginResultModel> Login([FromBody] LoginModel model)
    {
        var loginResult = await GetLoginResultAsync(
         model.UserName,
         model.Password,
         GetTenancyNameOrNull()
     );


        var refreshToken = CreateRefreshToken(
            await CreateJwtClaims(
                loginResult.Identity,
                loginResult.User,
                tokenType: TokenType.RefreshToken
            )
        );

        var accessToken = CreateAccessToken(
            await CreateJwtClaims(
                loginResult.Identity,
                loginResult.User,
                refreshTokenKey: refreshToken.key
            )
        );

        return new LoginResultModel
        {
            AccessToken = accessToken,
            ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds,
            RefreshToken = refreshToken.token,
            RefreshTokenExpireInSeconds = (int)_configuration.RefreshTokenExpiration.TotalSeconds,
            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
            UserId = loginResult.User.Id,
        };

    }


    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    public async Task<RefreshTokenResult> RefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        var (isRefreshTokenValid, principal) = await IsRefreshTokenValid(refreshToken);
        if (!isRefreshTokenValid)
        {
            throw new UserFriendlyException("无效的令牌");
        }

        try
        {
            var user = await _userManager.GetUserAsync(UserIdentifier.Parse(principal.Claims.First(x => x.Type == AppConsts.UserIdentifier).Value));

            if (user == null)
            {
                throw new UserFriendlyException("无效的用户");
            }

            //if (AllowOneConcurrentLoginPerUser())
            //{
            //    await _userManager.UpdateSecurityStampAsync(user);
            //    await _securityStampHandler.SetSecurityStampCacheItem(user.TenantId, user.Id, user.SecurityStamp);
            //}


            principal = await _claimsPrincipalFactory.CreateAsync(user);
            //var accessToken = CreateAccessToken(await CreateJwtClaims(principal.Identity as ClaimsIdentity, user));

            //return await Task.FromResult(new RefreshTokenResult(accessToken,GetEncryptedAccessToken(accessToken),(int)_configuration.AccessTokenExpiration.TotalSeconds));


            var accessToken = CreateAccessToken(await CreateJwtClaims(principal.Identity as ClaimsIdentity, user), _configuration.RefreshedAccessTokenExpiration);

            return await Task.FromResult(new RefreshTokenResult(
                accessToken,
                GetEncryptedAccessToken(accessToken),
                (int)_configuration.RefreshedAccessTokenExpiration.TotalSeconds)
            );
        }
        catch (UserFriendlyException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException("无效的令牌!", e);
        }
    }

    /// <summary>
    /// 注销
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ADTOSharpAuthorize]
    public async Task LogOut()
    {
        if (ADTOSharpSession.UserId != null)
        {
            var tokenValidityKeyInClaims = User.Claims.First(c => c.Type == AppConsts.TokenValidityKey);
            await RemoveTokenAsync(tokenValidityKeyInClaims.Value);

            var refreshTokenValidityKeyInClaims =
                User.Claims.FirstOrDefault(c => c.Type == AppConsts.RefreshTokenValidityKey);
            if (refreshTokenValidityKeyInClaims != null)
            {
                await RemoveTokenAsync(refreshTokenValidityKeyInClaims.Value);
            }

            if (AllowOneConcurrentLoginPerUser())
            {
                await _securityStampHandler.RemoveSecurityStampCacheItem(
                    ADTOSharpSession.TenantId,
                    ADTOSharpSession.GetUserId()
                );
            }
        }
    }

    /// <summary>
    /// 模拟登录
    /// </summary>
    /// <param name="impersonationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ImpersonatedAuthenticateResultModel> ImpersonatedAuthenticate(string impersonationToken)
    {
        var result = await _impersonationManager.GetImpersonatedUserAndIdentity(impersonationToken);
        var accessToken = CreateAccessToken(await CreateJwtClaims(result.Identity, result.User));

        return new ImpersonatedAuthenticateResultModel
        {
            AccessToken = accessToken,
            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
            ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds
        };
    }
    /// <summary>
    /// 代理帐号登录
    /// </summary>
    /// <param name="userDelegationId"></param>
    /// <param name="impersonationToken"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpPost]
    public async Task<ImpersonatedAuthenticateResultModel> DelegatedImpersonatedAuthenticate(Guid userDelegationId, string impersonationToken)
    {
        var result = await _impersonationManager.GetImpersonatedUserAndIdentity(impersonationToken);
        var userDelegation = await _userDelegationManager.GetAsync(userDelegationId);

        if (!userDelegation.IsCreatedByUser(result.User.Id))
        {
            throw new UserFriendlyException("User delegation error...");
        }

        var expiration = userDelegation.EndTime.Subtract(Clock.Now);
        var accessToken = CreateAccessToken(await CreateJwtClaims(result.Identity, result.User, expiration),
            expiration);

        return new ImpersonatedAuthenticateResultModel
        {
            AccessToken = accessToken,
            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
            ExpireInSeconds = (int)expiration.TotalSeconds
        };
    }
    /// <summary>
    /// 关联帐号登录
    /// </summary>
    /// <param name="switchAccountToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<SwitchedAccountAuthenticateResultModel> LinkedAccountAuthenticate(string switchAccountToken)
    {
        var result = await _userLinkManager.GetSwitchedUserAndIdentity(switchAccountToken);
        var accessToken = CreateAccessToken(await CreateJwtClaims(result.Identity, result.User));

        return new SwitchedAccountAuthenticateResultModel
        {
            AccessToken = accessToken,
            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
            ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds
        };
    }


    private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalLoginInfo)
    {
        string username;

        using (var providerManager = _externalLoginInfoManagerFactory.GetExternalLoginInfoManager(externalLoginInfo.Provider))
        {
            username = providerManager.Object.GetUserNameFromExternalAuthUserInfo(externalLoginInfo);
        }

        var user = await _userRegistrationManager.RegisterAsync(
            externalLoginInfo.Name,
            externalLoginInfo.EmailAddress ?? $"{username}@qq.com",
            username,
            await _userManager.CreateRandomPassword(),
            true,
            null
        );

        user.Logins = new List<UserLogin>
        {
            new UserLogin
            {
                LoginProvider = externalLoginInfo.Provider,
                ProviderKey = externalLoginInfo.ProviderKey,
                TenantId = user.TenantId
            }
        };

        await CurrentUnitOfWork.SaveChangesAsync();

        return user;
    }



    [HttpGet]
    public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
    {
        var allProviders = _externalAuthConfiguration.ExternalLoginInfoProviders
            .Select(infoProvider => infoProvider.GetExternalLoginInfo())
            .Where(IsSchemeEnabledOnTenant)
            .ToList();
        return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(allProviders);
    }

    /// <summary>
    /// 第三方社交登录,如微信小程序
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
    {
        var externalUser = await GetExternalUserInfo(model);

        var loginResult = await _logInManager.LoginAsync(
            new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider),
            GetTenancyNameOrNull()
        );

        switch (loginResult.Result)
        {
            case ADTOSharpLoginResultType.Success:
                {
                    // One Concurrent Login 
                    //if (AllowOneConcurrentLoginPerUser())
                    //{
                    //    await ResetSecurityStampForLoginResult(loginResult);
                    //}

                    var refreshToken = CreateRefreshToken(
                        await CreateJwtClaims(
                            loginResult.Identity,
                            loginResult.User,
                            tokenType: TokenType.RefreshToken
                        )
                    );

                    var accessToken = CreateAccessToken(
                        await CreateJwtClaims(
                            loginResult.Identity,
                            loginResult.User,
                            refreshTokenKey: refreshToken.key
                        )
                    );

                    var returnUrl = model.ReturnUrl;

                    if (model.SingleSignIn.HasValue && model.SingleSignIn.Value &&
                        loginResult.Result == ADTOSharpLoginResultType.Success)
                    {
                        loginResult.User.SetSignInToken();
                        returnUrl = AddSingleSignInParametersToReturnUrl(
                            model.ReturnUrl,
                            loginResult.User.SignInToken,
                            loginResult.User.Id,
                            loginResult.User.TenantId
                        );
                    }

                    return new ExternalAuthenticateResultModel
                    {
                        AccessToken = accessToken,
                        EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                        ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds,
                        ReturnUrl = returnUrl,
                        RefreshToken = refreshToken.token,
                        RefreshTokenExpireInSeconds = (int)_configuration.RefreshTokenExpiration.TotalSeconds
                    };
                }
            case ADTOSharpLoginResultType.UnknownExternalLogin:
                {

                    return new ExternalAuthenticateResultModel
                    {
                        ProviderKey = externalUser.ProviderKey,
                        UnknownExternalLogin = true,
                    };



                    /*
                    var newUser = await RegisterExternalUserAsync(externalUser);
                    if (!newUser.IsActive)
                    {
                        return new ExternalAuthenticateResultModel
                        {
                            WaitingForActivation = true
                        };
                    }

                    //Try to login again with newly registered user!
                    loginResult = await _logInManager.LoginAsync(
                        new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider),
                        GetTenancyNameOrNull()
                    );

                    if (loginResult.Result != LoginResultType.Success)
                    {
                        throw _loginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            externalUser.EmailAddress,
                            GetTenancyNameOrNull()
                        );
                    }

                    var refreshToken = CreateRefreshToken(await CreateJwtClaims(loginResult.Identity,
                        loginResult.User, tokenType: TokenType.RefreshToken)
                    );

                    var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity,
                        loginResult.User, refreshTokenKey: refreshToken.key));

                    return new ExternalAuthenticateResultModel
                    {
                        AccessToken = accessToken,
                        EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                        ExpireInSeconds = (int)_configuration.AccessTokenExpiration.TotalSeconds,
                        RefreshToken = refreshToken.token,
                        RefreshTokenExpireInSeconds = (int)_configuration.RefreshTokenExpiration.TotalSeconds
                    };
                    */
                }
            default:
                {
                    throw _loginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        externalUser.EmailAddress,
                        GetTenancyNameOrNull()
                    );
                }
        }
    }


    /// <summary>
    /// 为指定的用户,添加第三方登录, 在执行一次常规登录操作后,执行该方法,可以帮定常登录帐号与第三方帐号的关系
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpPost]
    public async Task<bool> AddUserExternalLoginAsync([FromBody] AddUserLoginModel input)
    {
        // _userManager.FindByLoginAsync();

        if (!ADTOSharpSession.UserId.HasValue)
        {
            throw new UserFriendlyException(L("NotValidateUser"));
        }

        var user = await _userManager.FindByIdAsync(ADTOSharpSession.GetUserId().ToString());

        if (user == null)
        {
            throw new UserFriendlyException(L("NotValidateUser"));
        }

        var loginUser = await _userManager.FindByLoginAsync(input.LoginProvider, input.ProviderKey);
        if (loginUser == null)
        {
            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo(input.LoginProvider, input.ProviderKey, input.DisplayName));
            if (!result.Succeeded)
            {
                throw new UserFriendlyException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }
            return true;
        }
        return false;
    }
    #endregion


}

