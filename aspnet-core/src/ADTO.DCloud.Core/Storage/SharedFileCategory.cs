
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Storage
{
    /// <summary>
    /// 文件目录
    /// </summary>
    public class SharedFileCategory : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 目录名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 父级目录
        /// </summary>
        public Guid? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        /// <summary>
        /// 父级目录
        /// </summary>
        public virtual SharedFileCategory? Parent { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }
    }
}
