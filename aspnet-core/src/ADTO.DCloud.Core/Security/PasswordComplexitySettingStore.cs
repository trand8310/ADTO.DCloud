using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Zero.Configuration;
using System.Threading.Tasks;

namespace ADTO.DCloud.Security;

/// <summary>
/// 密码复杂度设置
/// </summary>
public class PasswordComplexitySettingStore : IPasswordComplexitySettingStore, ITransientDependency
{
    private readonly ISettingManager _settingManager;

    public PasswordComplexitySettingStore(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<PasswordComplexitySetting> GetSettingsAsync()
    {
        return new PasswordComplexitySetting
        {
            RequireDigit = await _settingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit),
            RequireLowercase = await _settingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
            RequireNonAlphanumeric = await _settingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
            RequireUppercase = await _settingManager.GetSettingValueAsync<bool>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
            RequiredLength = await _settingManager.GetSettingValueAsync<int>(ADTOSharpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
        };
    }
}