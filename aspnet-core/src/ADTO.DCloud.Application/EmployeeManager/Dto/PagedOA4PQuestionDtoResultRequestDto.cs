

using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 4P测试题库信息
    /// </summary>
    public class PagedOA4PQuestionDtoResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }
    }
}