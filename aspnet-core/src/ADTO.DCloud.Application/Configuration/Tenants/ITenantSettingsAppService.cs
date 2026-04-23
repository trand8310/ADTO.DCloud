using System.Threading.Tasks;
using ADTO.DCloud.Configuration.Tenants.Dto;
using ADTOSharp.Application.Services;

namespace ADTO.DCloud.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearDarkLogo();
        
        Task ClearLightLogo();

        Task ClearCustomCss();
    }
}
