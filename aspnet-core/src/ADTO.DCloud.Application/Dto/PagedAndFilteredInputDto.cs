using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Dto;

public class PagedAndFilteredInputDto : IPagedResultRequest
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

    /// <summary>
    /// 过滤条件
    /// </summary>
    public string Filter { get; set; }
    
    /// <summary>
    /// 过滤条件
    /// </summary>
    public string Keyword { get; set; }


    public PagedAndFilteredInputDto()
    {
        PageSize = AppConsts.DefaultPageSize;
    }
}
