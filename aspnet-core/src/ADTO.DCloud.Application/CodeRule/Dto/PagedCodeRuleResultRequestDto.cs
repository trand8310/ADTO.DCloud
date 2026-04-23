using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.CodeRule.Dto
{
    public class PagedCodeRuleResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
