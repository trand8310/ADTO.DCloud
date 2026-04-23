using ADTOSharp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Dto;

public class PagedInputDto : IPagedResultRequest
{
    /// <summary>
    /// 每页记录数
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageSize { get; set; }
    /// <summary>
    /// 页码
    /// </summary>
    [Range(0, int.MaxValue)]
    public int PageNumber { get; set; }

    public PagedInputDto()
    {
        PageSize = AppConsts.DefaultPageSize;
    }
}
