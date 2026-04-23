

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 培训文件分类表
    /// </summary>
    [AutoMap(typeof(TrainingDocCategory))]
    public class TrainingDocCategoryDto: FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int Sord { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreateUserName { get; set; }
    }
}
