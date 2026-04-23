using ADTO.DCloud.Common.Dto;
using ADTO.DCloud.Editions.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;

namespace ADTO.DCloud.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false);

        Task<PagedResultDto<FindUsersOutputDto>> FindUsers(FindUsersInput input);

        GetDefaultEditionNameOutput GetDefaultEditionName();
    }
}