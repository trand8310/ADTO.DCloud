using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTO.DCloud.Timing.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Configuration;



namespace ADTO.DCloud.Timing;
/// <summary>
/// 时间服务,主要用于本地化
/// </summary>
public class TimingAppService : DCloudAppServiceBase, ITimingAppService
{
    private readonly ITimeZoneService _timeZoneService;

    public TimingAppService(ITimeZoneService timeZoneService)
    {
        _timeZoneService = timeZoneService;
    }

    public async Task<ListResultDto<NameValueDto>> GetTimezones(GetTimezonesInput input)
    {
        var timeZones = await GetTimezoneInfos(input.DefaultTimezoneScope);
        return new ListResultDto<NameValueDto>(timeZones);
    }

    public async Task<List<ComboboxItemDto>> GetTimezoneComboboxItems(GetTimezoneComboboxItemsInput input)
    {
        var timeZones = await GetTimezoneInfos(input.DefaultTimezoneScope);
        var timeZoneItems = new ListResultDto<ComboboxItemDto>(timeZones.Select(e => new ComboboxItemDto(e.Value, e.Name)).ToList()).Items.ToList();

        if (!string.IsNullOrEmpty(input.SelectedTimezoneId))
        {
            var selectedEdition = timeZoneItems.FirstOrDefault(e => e.Value == input.SelectedTimezoneId);
            if (selectedEdition != null)
            {
                selectedEdition.IsSelected = true;
            }
        }

        return timeZoneItems;
    }

    private async Task<List<NameValueDto>> GetTimezoneInfos(SettingScopes defaultTimezoneScope)
    {
        var defaultTimezoneId = await _timeZoneService.GetDefaultTimezoneAsync(defaultTimezoneScope, ADTOSharpSession.TenantId);

        var timeZones = _timeZoneService.GetWindowsTimezones();

        var defaultTimezoneName = $"{L("Default")} [{timeZones.FirstOrDefault(x => x.Value == defaultTimezoneId)?.Name ?? defaultTimezoneId}]";

        timeZones.Insert(0, new NameValueDto(defaultTimezoneName, string.Empty));
        return timeZones;
    }
}
