using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Notifications.Dto
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}