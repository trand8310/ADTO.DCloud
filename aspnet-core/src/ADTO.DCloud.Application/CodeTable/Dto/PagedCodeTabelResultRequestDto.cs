using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.CodeTable.Dto
{
    public class PagedCodeTabelResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
