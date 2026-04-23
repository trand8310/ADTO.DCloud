using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Training
{
    /// <summary>
    /// 培训文件分类表
    /// </summary>
    [Table("TrainingDocCategorys")]
    public class TrainingDocCategory: FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 排序值
        /// </summary>
        public int Sord { get; set; }
    }
}
