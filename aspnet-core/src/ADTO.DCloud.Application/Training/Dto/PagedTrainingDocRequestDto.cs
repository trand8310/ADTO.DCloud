
using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 培训文件库分页查询
    /// </summary>
    public class PagedTrainingDocRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public Guid? CategoryId { get; set; }

    }
}
