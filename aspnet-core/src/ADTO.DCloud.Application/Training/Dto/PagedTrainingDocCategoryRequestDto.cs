

using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 培训库类别分页查询
    /// </summary>
    public class PagedTrainingDocCategoryRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }
    }
}
