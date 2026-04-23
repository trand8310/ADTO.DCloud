using System.Threading.Tasks;
using ADTO.DCloud.Configuration.Host.Dto;
using ADTOSharp.Application.Services;

namespace ADTO.DCloud.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
