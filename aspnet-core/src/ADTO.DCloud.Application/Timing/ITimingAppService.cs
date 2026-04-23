using System.Collections.Generic;
using System.Threading.Tasks;
using ADTO.DCloud.Timing.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Timing
{
    public interface ITimingAppService : IApplicationService
    {
        Task<ListResultDto<NameValueDto>> GetTimezones(GetTimezonesInput input);

        Task<List<ComboboxItemDto>> GetTimezoneComboboxItems(GetTimezoneComboboxItemsInput input);
    }
}
