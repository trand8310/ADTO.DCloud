using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DataSource.Dto
{
    /// <summary>
    /// 数据源分页查询
    /// </summary>
    public class PagedDataSourceResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
