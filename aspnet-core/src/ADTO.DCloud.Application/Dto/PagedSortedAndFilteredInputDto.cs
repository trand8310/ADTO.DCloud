namespace ADTO.DCloud.Dto;

public class PagedSortedAndFilteredInputDto : PagedAndSortedInputDto
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string Filter { get; set; }
}
