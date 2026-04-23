using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Storage
{
    /// <summary>
    /// 共享文件
    /// </summary>
    public class SharedFileInfo : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 类别ID
        /// </summary>
        public Guid? CategoryId { get; set; }
        /// <summary>
        /// 文件类别
        /// </summary>
        [ForeignKey("CategoryId")]
        public SharedFileCategory? Category { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// 路径,相对根目录的路径
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalName { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// 描述及备注
        /// </summary>
        public string Description { get; set; }

    }
}
