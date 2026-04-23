

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 培训文件库
    /// </summary>
    [AutoMap(typeof(TrainingDoc))]
    public class TrainingDocDto: FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 文件标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文件类别Id
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreateUserName { get; set; }
    }
}
