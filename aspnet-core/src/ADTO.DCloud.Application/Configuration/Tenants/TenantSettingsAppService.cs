using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Configuration;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Extensions;
using ADTOSharp.Json;
using ADTOSharp.Net.Mail;
using ADTOSharp.Runtime.Security;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Configuration.Dto;
using ADTO.DCloud.Configuration.Host.Dto;
using ADTO.DCloud.Configuration.Tenants.Dto;
using ADTO.DCloud.Security;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Timing;
using ADTOSharp.Authorization;
using ADTOSharp.Zero.Configuration;

namespace ADTO.DCloud.Configuration.Tenants
{
    /// <summary>
    /// “ 系统配置”页面使用的应用程序服务。系统
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_Tenant_Settings)]
    public class TenantSettingsAppService : SettingsAppServiceBase, ITenantSettingsAppService
    {
        public IExternalLoginOptionsCacheManager ExternalLoginOptionsCacheManager { get; set; }

        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IBinaryObjectManager _binaryObjectManager;

        public TenantSettingsAppService(
            IMultiTenancyConfig multiTenancyConfig,
            ITimeZoneService timeZoneService,
            IEmailSender emailSender,
            IBinaryObjectManager binaryObjectManager,
            IAppConfigurationAccessor configurationAccessor
        ) : base(emailSender, configurationAccessor)
        {
            ExternalLoginOptionsCacheManager = NullExternalLoginOptionsCacheManager.Instance;

            _multiTenancyConfig = multiTenancyConfig;
            _timeZoneService = timeZoneService;
            _binaryObjectManager = binaryObjectManager;
        }

        #region Get Settings

        public async Task<TenantSettingsEditDto> GetAllSettings()
        {
            var settings = new TenantSettingsEditDto
            {
                UserManagement = await GetUserManagementSettingsAsync(),
                Security = await GetSecuritySettingsAsync(),
                OtherSettings = await GetOtherSettingsAsync(),
                Email = await GetEmailSettingsAsync(),
            };

            if (!_multiTenancyConfig.IsEnabled || Clock.SupportsMultipleTimezone)
            {
                settings.General = await GetGeneralSettingsAsync();
            }


            return settings;
        }


        private async Task<TenantEmailSettingsEditDto> GetEmailSettingsAsync()
        {
            var useHostDefaultEmailSettings =
                await SettingManager.GetSettingValueForTenantAsync<bool>(AppSettings.Email.UseHostDefaultEmailSettings,
                    ADTOSharpSession.GetTenantId());

            if (useHostDefaultEmailSettings)
            {
                return new TenantEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true
                };
            }

            var smtpPassword =
                await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Password,
                    ADTOSharpSession.GetTenantId());

            return new TenantEmailSettingsEditDto
            {
                UseHostDefaultEmailSettings = false,
                DefaultFromAddress =
                    await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromAddress,
                        ADTOSharpSession.GetTenantId()),
                DefaultFromDisplayName =
                    await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromDisplayName,
                        ADTOSharpSession.GetTenantId()),
                SmtpHost = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Host,
                    ADTOSharpSession.GetTenantId()),
                SmtpPort = await SettingManager.GetSettingValueForTenantAsync<int>(EmailSettingNames.Smtp.Port,
                    ADTOSharpSession.GetTenantId()),
                SmtpUserName =
                    await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.UserName,
                        ADTOSharpSession.GetTenantId()),
                SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                SmtpDomain =
                    await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Domain,
                        ADTOSharpSession.GetTenantId()),
                SmtpEnableSsl =
                    await SettingManager.GetSettingValueForTenantAsync<bool>(EmailSettingNames.Smtp.EnableSsl,
                        ADTOSharpSession.GetTenantId()),
                SmtpUseDefaultCredentials =
                    await SettingManager.GetSettingValueForTenantAsync<bool>(
                        EmailSettingNames.Smtp.UseDefaultCredentials, ADTOSharpSession.GetTenantId())
            };
        }

        private async Task<GeneralSettingsEditDto> GetGeneralSettingsAsync()
        {
            var settings = new GeneralSettingsEditDto();

            if (Clock.SupportsMultipleTimezone)
            {
                var timezone =
                    await SettingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone,
                        ADTOSharpSession.GetTenantId());

                settings.Timezone = timezone;
                settings.TimezoneForComparison = timezone;
            }

            var defaultTimeZoneId =
                await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Tenant, ADTOSharpSession.TenantId);

            if (settings.Timezone == defaultTimeZoneId)
            {
                settings.Timezone = string.Empty;
            }

            return settings;
        }

        private async Task<TenantUserManagementSettingsEditDto> GetUserManagementSettingsAsync()
        {
            return new TenantUserManagementSettingsEditDto
            {
                AllowSelfRegistration =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowSelfRegistration),
                IsNewRegisteredUserActiveByDefault =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                        .IsNewRegisteredUserActiveByDefault),
                IsEmailConfirmationRequiredForLogin =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .IsEmailConfirmationRequiredForLogin),
                UseCaptchaOnRegistration =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                        .UseCaptchaOnRegistration),
                UseCaptchaOnLogin =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.UseCaptchaOnLogin),
                IsCookieConsentEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsCookieConsentEnabled),
                IsQuickThemeSelectEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(
                        AppSettings.UserManagement.IsQuickThemeSelectEnabled),
                AllowUsingGravatarProfilePicture =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                        .AllowUsingGravatarProfilePicture),
                SessionTimeOutSettings = new SessionTimeOutSettingsEditDto()
                {
                    IsEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                        .SessionTimeOut.IsEnabled),
                    TimeOutSecond =
                        await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut
                            .TimeOutSecond),
                    ShowTimeOutNotificationSecond =
                        await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut
                            .ShowTimeOutNotificationSecond),
                    ShowLockScreenWhenTimedOut =
                        await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.SessionTimeOut
                            .ShowLockScreenWhenTimedOut)
                }
            };
        }

        private async Task<SecuritySettingsEditDto> GetSecuritySettingsAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireUppercase),
                RequiredLength =
                    await SettingManager.GetSettingValueAsync<int>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity
                        .RequiredLength)
            };

            var defaultPasswordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase =
                    await SettingManager.GetSettingValueForApplicationAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireUppercase),
                RequiredLength =
                    await SettingManager.GetSettingValueForApplicationAsync<int>(ADTOSharpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequiredLength)
            };

            return new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings =
                    passwordComplexitySetting.Equals(defaultPasswordComplexitySetting),
                PasswordComplexity = passwordComplexitySetting,
                DefaultPasswordComplexity = defaultPasswordComplexitySetting,
                UserLockOut = await GetUserLockOutSettingsAsync(),
                TwoFactorLogin = await GetTwoFactorLoginSettingsAsync(),
                AllowOneConcurrentLoginPerUser = await GetOneConcurrentLoginPerUserSetting()
            };
        }

        private async Task<TenantBillingSettingsEditDto> GetBillingSettingsAsync()
        {
            return new TenantBillingSettingsEditDto()
            {
                LegalName = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingLegalName),
                Address = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingAddress),
                TaxVatNo = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingTaxVatNo)
            };
        }

        private async Task<TenantOtherSettingsEditDto> GetOtherSettingsAsync()
        {
            return new TenantOtherSettingsEditDto()
            {
                IsQuickThemeSelectEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(
                        AppSettings.UserManagement.IsQuickThemeSelectEnabled)
            };
        }

        private async Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync()
        {
            return new UserLockOutSettingsEditDto
            {
                IsEnabled = await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                    .UserLockOut.IsEnabled),
                MaxFailedAccessAttemptsBeforeLockout =
                    await SettingManager.GetSettingValueAsync<int>(ADTOSharpZeroSettingNames.UserManagement.UserLockOut
                        .MaxFailedAccessAttemptsBeforeLockout),
                DefaultAccountLockoutSeconds =
                    await SettingManager.GetSettingValueAsync<int>(ADTOSharpZeroSettingNames.UserManagement.UserLockOut
                        .DefaultAccountLockoutSeconds)
            };
        }

        private Task<bool> IsTwoFactorLoginEnabledForApplicationAsync()
        {
            return SettingManager.GetSettingValueForApplicationAsync<bool>(ADTOSharpZeroSettingNames.UserManagement
                .TwoFactorLogin.IsEnabled);
        }

        private async Task<TwoFactorLoginSettingsEditDto> GetTwoFactorLoginSettingsAsync()
        {
            var settings = new TwoFactorLoginSettingsEditDto
            {
                IsEnabledForApplication = await IsTwoFactorLoginEnabledForApplicationAsync()
            };

            if (_multiTenancyConfig.IsEnabled && !settings.IsEnabledForApplication)
            {
                return settings;
            }

            settings.IsEnabled =
                await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin
                    .IsEnabled);
            settings.IsRememberBrowserEnabled =
                await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin
                    .IsRememberBrowserEnabled);

            if (!_multiTenancyConfig.IsEnabled)
            {
                settings.IsEmailProviderEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin
                        .IsEmailProviderEnabled);
                settings.IsSmsProviderEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin
                        .IsSmsProviderEnabled);
                settings.IsGoogleAuthenticatorEnabled =
                    await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.TwoFactorLogin
                        .IsGoogleAuthenticatorEnabled);
            }

            return settings;
        }

        private async Task<bool> GetOneConcurrentLoginPerUserSetting()
        {
            return await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement
                .AllowOneConcurrentLoginPerUser);
        }


        #endregion

        #region Update Settings

        public async Task UpdateAllSettings(TenantSettingsEditDto input)
        {
            await UpdateUserManagementSettingsAsync(input.UserManagement);
            await UpdateSecuritySettingsAsync(input.Security);
            await UpdateEmailSettingsAsync(input.Email);

            //Time Zone
            if (Clock.SupportsMultipleTimezone)
            {
                if (input.General.Timezone.IsNullOrEmpty())
                {
                    var defaultValue =
                        await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Tenant, ADTOSharpSession.TenantId);
                    await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                        TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                        TimingSettingNames.TimeZone, input.General.Timezone);
                }
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                await UpdateOtherSettingsAsync(input.OtherSettings);

                input.ValidateHostSettings();
            }
        }

        private async Task UpdateOtherSettingsAsync(TenantOtherSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.IsQuickThemeSelectEnabled,
                input.IsQuickThemeSelectEnabled.ToString().ToLowerInvariant()
            );
        }

        private async Task UpdateBillingSettingsAsync(TenantBillingSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                AppSettings.TenantManagement.BillingLegalName, input.LegalName);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                AppSettings.TenantManagement.BillingAddress, input.Address);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                AppSettings.TenantManagement.BillingTaxVatNo, input.TaxVatNo);
        }



        private async Task UpdateEmailSettingsAsync(TenantEmailSettingsEditDto input)
        {
            if (_multiTenancyConfig.IsEnabled && !DCloudConsts.AllowTenantsToChangeEmailSettings)
            {
                return;
            }

            var useHostDefaultEmailSettings = _multiTenancyConfig.IsEnabled && input.UseHostDefaultEmailSettings;

            if (useHostDefaultEmailSettings)
            {
                var smtpPassword =
                    await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password);

                input = new TenantEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true,
                    DefaultFromAddress =
                        await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                    DefaultFromDisplayName =
                        await SettingManager.GetSettingValueForApplicationAsync(
                            EmailSettingNames.DefaultFromDisplayName),
                    SmtpHost = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                    SmtpPort =
                        await SettingManager.GetSettingValueForApplicationAsync<int>(EmailSettingNames.Smtp.Port),
                    SmtpUserName =
                        await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                    SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                    SmtpDomain = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Domain),
                    SmtpEnableSsl =
                        await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.EnableSsl),
                    SmtpUseDefaultCredentials =
                        await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp
                            .UseDefaultCredentials)
                };
            }

            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                AppSettings.Email.UseHostDefaultEmailSettings,
                useHostDefaultEmailSettings.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                EmailSettingNames.DefaultFromAddress, input.DefaultFromAddress);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                EmailSettingNames.DefaultFromDisplayName, input.DefaultFromDisplayName);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(), EmailSettingNames.Smtp.Host,
                input.SmtpHost);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(), EmailSettingNames.Smtp.Port,
                input.SmtpPort.ToString(CultureInfo.InvariantCulture));
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(), EmailSettingNames.Smtp.UserName,
                input.SmtpUserName);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(), EmailSettingNames.Smtp.Password,
                SimpleStringCipher.Instance.Encrypt(input.SmtpPassword));
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(), EmailSettingNames.Smtp.Domain,
                input.SmtpDomain);
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(), EmailSettingNames.Smtp.EnableSsl,
                input.SmtpEnableSsl.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                EmailSettingNames.Smtp.UseDefaultCredentials,
                input.SmtpUseDefaultCredentials.ToString().ToLowerInvariant());
        }

        private async Task UpdateUserManagementSettingsAsync(TenantUserManagementSettingsEditDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.AllowSelfRegistration,
                settings.AllowSelfRegistration.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault,
                settings.IsNewRegisteredUserActiveByDefault.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                settings.IsEmailConfirmationRequiredForLogin.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.UseCaptchaOnRegistration,
                settings.UseCaptchaOnRegistration.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.UseCaptchaOnLogin,
                settings.UseCaptchaOnLogin.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.IsCookieConsentEnabled,
                settings.IsCookieConsentEnabled.ToString().ToLowerInvariant()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.AllowUsingGravatarProfilePicture,
                settings.AllowUsingGravatarProfilePicture.ToString().ToLowerInvariant()
            );

            await UpdateUserManagementSessionTimeOutSettingsAsync(settings.SessionTimeOutSettings);
        }

        private async Task UpdateUserManagementSessionTimeOutSettingsAsync(SessionTimeOutSettingsEditDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.IsEnabled,
                settings.IsEnabled.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.TimeOutSecond,
                settings.TimeOutSecond.ToString()
            );
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond,
                settings.ShowTimeOutNotificationSecond.ToString()
            );
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut,
                settings.ShowLockScreenWhenTimedOut.ToString()
            );
        }

        private async Task UpdateSecuritySettingsAsync(SecuritySettingsEditDto settings)
        {
            if (settings.UseDefaultPasswordComplexitySettings)
            {
                await UpdatePasswordComplexitySettingsAsync(settings.DefaultPasswordComplexity);
            }
            else
            {
                if (settings.PasswordComplexity.RequiredLength < settings.PasswordComplexity.AllowedMinimumLength)
                {
                    throw new UserFriendlyException(L("AllowedMinimumLength",
                        settings.PasswordComplexity.AllowedMinimumLength));
                }

                await UpdatePasswordComplexitySettingsAsync(settings.PasswordComplexity);
            }

            await UpdateUserLockOutSettingsAsync(settings.UserLockOut);
            await UpdateTwoFactorLoginSettingsAsync(settings.TwoFactorLogin);
            await UpdateOneConcurrentLoginPerUserSettingAsync(settings.AllowOneConcurrentLoginPerUser);
        }

        private async Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                settings.RequireDigit.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                settings.RequireLowercase.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                settings.RequireNonAlphanumeric.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                settings.RequireUppercase.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                settings.RequiredLength.ToString()
            );
        }

        private async Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.UserLockOut.IsEnabled,
                settings.IsEnabled.ToString().ToLowerInvariant()
            );
            
            if (settings.DefaultAccountLockoutSeconds.HasValue)
            {
                await SettingManager.ChangeSettingForTenantAsync(
                    ADTOSharpSession.GetTenantId(),
                    ADTOSharpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                    settings.DefaultAccountLockoutSeconds.ToString()
                );
            }

            if (settings.MaxFailedAccessAttemptsBeforeLockout.HasValue)
            {
                await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                    ADTOSharpZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                    settings.MaxFailedAccessAttemptsBeforeLockout.ToString()
                );
            }
        }

        private async Task UpdateTwoFactorLoginSettingsAsync(TwoFactorLoginSettingsEditDto settings)
        {
            if (_multiTenancyConfig.IsEnabled &&
                !await IsTwoFactorLoginEnabledForApplicationAsync()) //Two factor login can not be used by tenants if disabled by the host
            {
                return;
            }

            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled,
                settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled,
                settings.IsRememberBrowserEnabled.ToString().ToLowerInvariant());

            if (!_multiTenancyConfig.IsEnabled)
            {
                //These settings can only be changed by host, in a multitenant application.
                await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                    ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsEmailProviderEnabled,
                    settings.IsEmailProviderEnabled.ToString().ToLowerInvariant());
                await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                    ADTOSharpZeroSettingNames.UserManagement.TwoFactorLogin.IsSmsProviderEnabled,
                    settings.IsSmsProviderEnabled.ToString().ToLowerInvariant());
                await SettingManager.ChangeSettingForTenantAsync(ADTOSharpSession.GetTenantId(),
                    AppSettings.UserManagement.TwoFactorLogin.IsGoogleAuthenticatorEnabled,
                    settings.IsGoogleAuthenticatorEnabled.ToString().ToLowerInvariant());
            }
        }

        private async Task UpdateOneConcurrentLoginPerUserSettingAsync(bool allowOneConcurrentLoginPerUser)
        {
            if (_multiTenancyConfig.IsEnabled)
            {
                return;
            }

            await SettingManager.ChangeSettingForApplicationAsync(
                AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, allowOneConcurrentLoginPerUser.ToString());
        }


        #endregion

        #region Logo & Css

        public async Task ClearDarkLogo()
        {
            var tenant = await GetCurrentTenantAsync();

            if (!tenant.HasDarkLogo())
            {
                return;
            }

            var logoObject = await _binaryObjectManager.GetOrNullAsync(tenant.DarkLogoId.Value);
            if (logoObject != null)
            {
                await _binaryObjectManager.DeleteAsync(tenant.DarkLogoId.Value);
            }

            tenant.ClearDarkLogo();
        }
        
        public async Task ClearLightLogo()
        {
            var tenant = await GetCurrentTenantAsync();

            if (!tenant.HasLightLogo())
            {
                return;
            }

            var logoObject = await _binaryObjectManager.GetOrNullAsync(tenant.LightLogoId.Value);
            if (logoObject != null)
            {
                await _binaryObjectManager.DeleteAsync(tenant.LightLogoId.Value);
            }

            tenant.ClearLightLogo();
        }

        public async Task ClearCustomCss()
        {
            var tenant = await GetCurrentTenantAsync();

            if (!tenant.CustomCssId.HasValue)
            {
                return;
            }

            var cssObject = await _binaryObjectManager.GetOrNullAsync(tenant.CustomCssId.Value);
            if (cssObject != null)
            {
                await _binaryObjectManager.DeleteAsync(tenant.CustomCssId.Value);
            }

            tenant.CustomCssId = null;
        }

        #endregion
    }
}
