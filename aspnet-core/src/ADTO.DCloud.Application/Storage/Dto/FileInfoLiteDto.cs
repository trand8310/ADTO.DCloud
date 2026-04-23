

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// 文件
    /// </summary>
    [AutoMapFrom(typeof(SharedFileInfo))]
    public class FileInfoLiteDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        public FileCategoryLiteDto Category { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// 
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

        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }
        //public string Url { get; set; }
        //public UserSimpleDto? CreatorUser { get; set; }
        //public UserSimpleDto? LastModifierUser { get; set; }
    }
}
