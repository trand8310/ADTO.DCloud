using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Authorization;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Configuration.Dto;
using ADTO.DCloud.UiCustomization;
using ADTO.DCloud.Infrastructure;



namespace ADTO.DCloud.Configuration;

/// <summary>
/// 主题设置服务,配置前端使用
/// </summary>
[ADTOSharpAuthorize]
public class UiCustomizationSettingsAppService : DCloudAppServiceBase, IUiCustomizationSettingsAppService
{
    private readonly SettingManager _settingManager;
    private readonly IIocResolver _iocResolver;
    private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;

    public UiCustomizationSettingsAppService(
        SettingManager settingManager,
        IIocResolver iocResolver,
        IUiThemeCustomizerFactory uiThemeCustomizerFactory
    )
    {
        _settingManager = settingManager;
        _iocResolver = iocResolver;
        _uiThemeCustomizerFactory = uiThemeCustomizerFactory;
    }

    public async Task<List<ThemeSettingsDto>> GetUiManagementSettings()
    {
        var settings = new List<ThemeSettingsDto>();
        var themeCustomizers = _iocResolver.ResolveAll<IUiCustomizer>();

        foreach (var themeUiCustomizer in themeCustomizers)
        {
            var themeSettings = await themeUiCustomizer.GetUiSettings();
            settings.Add(themeSettings.BaseSettings);
        }

        return settings;
    }

    public async Task ChangeThemeWithDefaultValues(string themeName)
    {
        var settings = (await GetUiManagementSettings()).FirstOrDefault(s => s.Theme == themeName);

        var hasUiCustomizationPagePermission = await PermissionChecker.IsGrantedAsync(PermissionNames.Pages_Administration_UiCustomization);

        if (hasUiCustomizationPagePermission)
        {
            await UpdateDefaultUiManagementSettings(settings);
        }
        else
        {
            await UpdateUiManagementSettings(settings);
        }
    }

    public async Task UpdateUiManagementSettings(ThemeSettingsDto settings)
    {
        var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(settings.Theme);
        await themeCustomizer.UpdateUserUiManagementSettingsAsync(ADTOSharpSession.ToUserIdentifier(), settings);
    }

    public async Task UpdateDefaultUiManagementSettings(ThemeSettingsDto settings)
    {
        var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(settings.Theme);

        if (ADTOSharpSession.TenantId.HasValue)
        {
            await themeCustomizer.UpdateTenantUiManagementSettingsAsync(ADTOSharpSession.TenantId.Value, settings, ADTOSharpSession.ToUserIdentifier());
        }
        else
        {
            await themeCustomizer.UpdateApplicationUiManagementSettingsAsync(settings, ADTOSharpSession.ToUserIdentifier());
        }
    }

    public async Task UseSystemDefaultSettings()
    {
        if (ADTOSharpSession.TenantId.HasValue)
        {
            var theme = await _settingManager.GetSettingValueForTenantAsync(AppSettings.UiManagement.Theme, ADTOSharpSession.TenantId.Value);
            var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(theme);
            var settings = await themeCustomizer.GetTenantUiCustomizationSettings(ADTOSharpSession.TenantId.Value);
            await themeCustomizer.UpdateUserUiManagementSettingsAsync(ADTOSharpSession.ToUserIdentifier(), settings);
        }
        else
        {
            var theme = await _settingManager.GetSettingValueForApplicationAsync(AppSettings.UiManagement.Theme);
            var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(theme);
            var settings = await themeCustomizer.GetHostUiManagementSettings();
            await themeCustomizer.UpdateUserUiManagementSettingsAsync(ADTOSharpSession.ToUserIdentifier(), settings);
        }
    }

    public async Task ChangeDarkModeOfCurrentTheme(bool isDarkModeActive)
    {
        var user = ADTOSharpSession.ToUserIdentifier();
        var theme = await _settingManager.GetSettingValueForUserAsync(AppSettings.UiManagement.Theme, user);

        var themeCustomizer = _uiThemeCustomizerFactory.GetUiCustomizer(theme);
        await themeCustomizer.UpdateDarkModeSettingsAsync(user, isDarkModeActive);
    }
}
