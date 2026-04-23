using System;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities.Auditing;


namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 培训文件库
    /// </summary>
    [Table("TrainingDocs")]
    public class TrainingDoc : FullAuditedEntity<Guid>
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
        /// 培训文件分类表Id
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual TrainingDocCategory Category { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
