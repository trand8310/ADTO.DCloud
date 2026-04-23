using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.Configuration.Dto;
using ADTOSharp.Application.Services;

namespace ADTO.DCloud.Configuration
{
    public interface IUiCustomizationSettingsAppService : IApplicationService
    {
        Task<List<ThemeSettingsDto>> GetUiManagementSettings();

        Task UpdateUiManagementSettings(ThemeSettingsDto settings);

        Task UpdateDefaultUiManagementSettings(ThemeSettingsDto settings);

        Task UseSystemDefaultSettings();

        Task ChangeDarkModeOfCurrentTheme(bool isDarkModeActive);
    }
}
