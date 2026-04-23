

using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 员工培训记录分页查询条件
    /// </summary>
    public class PagedEmployeesTrainingArchivesRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 培训月份
        /// </summary>
        public string TrainingMonth { get; set; }
    }
}
