using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Authorization.Users.Dto;

namespace ADTO.DCloud.Authorization.Users
{
    public interface IUserLoginAppService : IApplicationService
    {
        Task<PagedResultDto<UserLoginAttemptDto>> GetUserLoginAttempts(GetLoginAttemptsInput input);
    }
}
