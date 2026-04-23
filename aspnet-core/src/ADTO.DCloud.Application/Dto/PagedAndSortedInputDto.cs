using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Dto;

public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
{
    /// <summary>
    /// 排序字段
    /// </summary>
    public string Sorting { get; set; }

    public PagedAndSortedInputDto()
    {
        PageSize = AppConsts.DefaultPageSize;
    }
}
