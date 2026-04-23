using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Features;
using ADTOSharp.Application.Navigation;
using ADTOSharp.Authorization;
using ADTOSharp.Configuration;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing;
using ADTOSharp.Timing.Timezone;
using ADTOSharp.Web.Models.ADTOSharpUserConfiguration;
using ADTOSharp.Web.Security.AntiForgery;
using System.Linq;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using System.Globalization;

namespace ADTOSharp.Web.Configuration
{
    public class ADTOSharpUserConfigurationBuilder : ITransientDependency
    {
        private readonly IADTOSharpStartupConfiguration _startupConfiguration;

        protected IMultiTenancyConfig MultiTenancyConfig { get; }
        protected ILanguageManager LanguageManager { get; }
        protected ILocalizationManager LocalizationManager { get; }
        protected IFeatureManager FeatureManager { get; }
        protected IFeatureChecker FeatureChecker { get; }
        protected IPermissionManager PermissionManager { get; }
        protected IUserNavigationManager UserNavigationManager { get; }
        protected ISettingDefinitionManager SettingDefinitionManager { get; }
        protected ISettingManager SettingManager { get; }
        protected IADTOSharpAntiForgeryConfiguration ADTOSharpAntiForgeryConfiguration { get; }
        protected IADTOSharpSession ADTOSharpSession { get; }
        protected IPermissionChecker PermissionChecker { get; }
        protected Dictionary<string, object> CustomDataConfig { get; }

        private readonly IIocResolver _iocResolver;

        public ADTOSharpUserConfigurationBuilder(
            IMultiTenancyConfig multiTenancyConfig,
            ILanguageManager languageManager,
            ILocalizationManager localizationManager,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker,
            IPermissionManager permissionManager,
            IUserNavigationManager userNavigationManager,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            IADTOSharpAntiForgeryConfiguration adtoAntiForgeryConfiguration,
            IADTOSharpSession adtoSession,
            IPermissionChecker permissionChecker,
            IIocResolver iocResolver,
            IADTOSharpStartupConfiguration startupConfiguration)
        {
            MultiTenancyConfig = multiTenancyConfig;
            LanguageManager = languageManager;
            LocalizationManager = localizationManager;
            FeatureManager = featureManager;
            FeatureChecker = featureChecker;
            PermissionManager = permissionManager;
            UserNavigationManager = userNavigationManager;
            SettingDefinitionManager = settingDefinitionManager;
            SettingManager = settingManager;
            ADTOSharpAntiForgeryConfiguration = adtoAntiForgeryConfiguration;
            ADTOSharpSession = adtoSession;
            PermissionChecker = permissionChecker;
            _iocResolver = iocResolver;
            _startupConfiguration = startupConfiguration;

            CustomDataConfig = new Dictionary<string, object>();
        }

        public virtual async Task<ADTOSharpUserConfigurationDto> GetAll()
        {
            return new ADTOSharpUserConfigurationDto
            {
                MultiTenancy = GetUserMultiTenancyConfig(),
                Session = GetUserSessionConfig(),
                Localization = GetUserLocalizationConfig(),
                Features = await GetUserFeaturesConfig(),
                Auth = await GetUserAuthConfig(),
                Nav = await GetUserNavConfig(),
                Setting = await GetUserSettingConfig(),
                Clock = GetUserClockConfig(),
                Timing = await GetUserTimingConfig(),
                Security = GetUserSecurityConfig(),
                Custom = _startupConfiguration.GetCustomConfig()
            };
        }

        protected virtual ADTOSharpMultiTenancyConfigDto GetUserMultiTenancyConfig()
        {
            return new ADTOSharpMultiTenancyConfigDto
            {
                IsEnabled = MultiTenancyConfig.IsEnabled,
                IgnoreFeatureCheckForHostUsers = MultiTenancyConfig.IgnoreFeatureCheckForHostUsers
            };
        }

        protected virtual ADTOSharpUserSessionConfigDto GetUserSessionConfig()
        {
            return new ADTOSharpUserSessionConfigDto
            {
                UserId = ADTOSharpSession.UserId,
                TenantId = ADTOSharpSession.TenantId,
                ImpersonatorUserId = ADTOSharpSession.ImpersonatorUserId,
                ImpersonatorTenantId = ADTOSharpSession.ImpersonatorTenantId,
                MultiTenancySide = ADTOSharpSession.MultiTenancySide
            };
        }

        protected virtual ADTOSharpUserLocalizationConfigDto GetUserLocalizationConfig()
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            var languages = LanguageManager.GetActiveLanguages();

            var config = new ADTOSharpUserLocalizationConfigDto
            {
                CurrentCulture = new ADTOSharpUserCurrentCultureConfigDto
                {
                    Name = currentCulture.Name,
                    DisplayName = currentCulture.DisplayName
                },
                Languages = languages.ToList()
            };

            if (languages.Count > 0)
            {
                config.CurrentLanguage = LanguageManager.CurrentLanguage;
            }

            var sources = LocalizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
            config.Sources = sources.Select(s => new ADTOSharpLocalizationSourceDto
            {
                Name = s.Name,
                Type = s.GetType().Name
            }).ToList();

            config.Values = new Dictionary<string, Dictionary<string, string>>();
            foreach (var source in sources)
            {
                var stringValues = source.GetAllStrings(currentCulture).OrderBy(s => s.Name).ToList();
                var stringDictionary = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value);
                config.Values.Add(source.Name, stringDictionary);
            }

            return config;
        }

        protected virtual async Task<ADTOSharpUserFeatureConfigDto> GetUserFeaturesConfig()
        {
            var config = new ADTOSharpUserFeatureConfigDto()
            {
                AllFeatures = new Dictionary<string, ADTOSharpStringValueDto>()
            };

            var allFeatures = FeatureManager.GetAll().ToList();

            if (ADTOSharpSession.TenantId.HasValue)
            {
                var currentTenantId = ADTOSharpSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    var value = await FeatureChecker.GetValueAsync(currentTenantId, feature.Name);
                    config.AllFeatures.Add(feature.Name, new ADTOSharpStringValueDto
                    {
                        Value = value
                    });
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    config.AllFeatures.Add(feature.Name, new ADTOSharpStringValueDto
                    {
                        Value = feature.DefaultValue
                    });
                }
            }

            return config;
        }

        protected virtual async Task<ADTOSharpUserAuthConfigDto> GetUserAuthConfig()
        {
            var config = new ADTOSharpUserAuthConfigDto();

            var allPermissionNames = PermissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (ADTOSharpSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await PermissionChecker.IsGrantedAsync(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }

            config.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
            config.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");

            return config;
        }

        protected virtual async Task<ADTOSharpUserNavConfigDto> GetUserNavConfig()
        {
            var userMenus = await UserNavigationManager.GetMenusAsync(ADTOSharpSession.ToUserIdentifier());
            return new ADTOSharpUserNavConfigDto
            {
                Menus = userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu)
            };
        }

        protected virtual async Task<ADTOSharpUserSettingConfigDto> GetUserSettingConfig()
        {
            var config = new ADTOSharpUserSettingConfigDto
            {
                Values = new Dictionary<string, string>()
            };

            var settings = await SettingManager.GetAllSettingValuesAsync(SettingScopes.All);

            using (var scope = _iocResolver.CreateScope())
            {
                foreach (var settingValue in settings)
                {
                    if (!await SettingDefinitionManager.GetSettingDefinition(settingValue.Name).ClientVisibilityProvider
                        .CheckVisible(scope))
                    {
                        continue;
                    }

                    config.Values.Add(settingValue.Name, settingValue.Value);
                }
            }

            return config;
        }

        protected virtual ADTOSharpUserClockConfigDto GetUserClockConfig()
        {
            return new ADTOSharpUserClockConfigDto
            {
                Provider = Clock.Provider.GetType().Name.ToCamelCase()
            };
        }

        protected virtual async Task<ADTOSharpUserTimingConfigDto> GetUserTimingConfig()
        {
            var timezoneId = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimezoneHelper.FindTimeZoneInfo(timezoneId);

            return new ADTOSharpUserTimingConfigDto
            {
                TimeZoneInfo = new ADTOSharpUserTimeZoneConfigDto
                {
                    Windows = new ADTOSharpUserWindowsTimeZoneConfigDto
                    {
                        TimeZoneId = timezoneId,
                        BaseUtcOffsetInMilliseconds = timezone.BaseUtcOffset.TotalMilliseconds,
                        CurrentUtcOffsetInMilliseconds = timezone.GetUtcOffset(Clock.Now).TotalMilliseconds,
                        IsDaylightSavingTimeNow = timezone.IsDaylightSavingTime(Clock.Now)
                    },
                    Iana = new ADTOSharpUserIanaTimeZoneConfigDto
                    {
                        TimeZoneId = TimezoneHelper.WindowsToIana(timezoneId)
                    }
                }
            };
        }

        protected virtual ADTOSharpUserSecurityConfigDto GetUserSecurityConfig()
        {
            return new ADTOSharpUserSecurityConfigDto
            {
                AntiForgery = new ADTOSharpUserAntiForgeryConfigDto
                {
                    TokenCookieName = ADTOSharpAntiForgeryConfiguration.TokenCookieName,
                    TokenHeaderName = ADTOSharpAntiForgeryConfiguration.TokenHeaderName
                }
            };
        }
    }
}
