

using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 培训记录统计查询条件
    /// </summary>
    public class PagedTrainingArchivesCountRequestDto : PagedResultRequestDto
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
