

using System;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// 保存文件信息Dto
    /// </summary>
    public class SharedFileInfoDto
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
