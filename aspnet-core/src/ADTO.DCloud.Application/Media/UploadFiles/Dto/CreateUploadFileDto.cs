using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Media.UploadFiles.Dto
{
    /// <summary>
    /// 添加文件
    /// </summary>
    [AutoMapTo(typeof(UploadFile))]
    public class CreateUploadFileDto
    {
        /// <summary>
        /// 附件夹主键（单属性上传多张图片）
        /// </summary>
        public Guid FolderId { get; set; }

        /// <summary>
        /// 具体业务对象Id（存具体关联对象Id，例如客户Id、项目Id）
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 具体实体Id（存具体业务表Id,例如项目的跟进记录Id、合同记录Id）
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExtensions { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// 所属文件分类Id
        /// </summary>
        public Guid? FileTypeId { get; set; }

    }
}
