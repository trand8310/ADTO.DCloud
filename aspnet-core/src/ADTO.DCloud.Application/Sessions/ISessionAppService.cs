using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTO.DCloud.Sessions.Dto;
using ADTOSharp.Web.Models.ADTOSharpUserConfiguration;

namespace ADTO.DCloud.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentSessionOutput> GetCurrentSessionAsync();
        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
        Task<ADTOSharpUserAuthConfigDto> GetUserAuthConfig();
        ADTOSharpUserLocalizationConfigDto GetUserLocalizationConfig();
    }
}
