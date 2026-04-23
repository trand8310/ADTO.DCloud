using System;
using System.Collections.Generic;


namespace ADTO.DCloud.Storage.Dto
{
    public class UploadFileFolderDto
    {
        /// <summary>
        /// 文件类别ID
        /// </summary>
        public Guid? CategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FolderDto> FolderDtos { get; set; }
    }
    public class FolderDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; } = 0;
        /// <summary>
        /// 文件类别
        /// </summary>
        public Guid CategoryId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 路径,相对根目录的路径
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 原始文件名
        /// </summary>
        public string OriginalName { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        public List<FolderDto> Children { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileExtension { get; set; }
    }
}
