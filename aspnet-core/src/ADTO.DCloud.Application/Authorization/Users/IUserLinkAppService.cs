using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Authorization.Users.Dto;

namespace ADTO.DCloud.Authorization.Users
{
    public interface IUserLinkAppService : IApplicationService
    {
        Task LinkToUser(LinkToUserInput linkToUserInput);

        Task<PagedResultDto<LinkedUserDto>> GetLinkedUsers(GetLinkedUsersInput input);

        Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsers();

        Task UnlinkUser(UnlinkUserInput input);
    }
}
