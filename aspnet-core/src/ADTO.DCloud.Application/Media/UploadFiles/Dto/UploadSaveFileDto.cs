using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Media.UploadFiles.Dto
{
    /// <summary>
    /// 上传保存文件Dto
    /// </summary>
    public class UploadSaveFileDto
    {
        public UploadSaveFileDto()
        {
            FileTokens = new List<string>();
        }

        /// <summary>
        /// 内存中上传的图片，非表单方式不需要传
        /// </summary>
        public List<string> FileTokens { get; set; }

        /// <summary>
        /// 附件夹主键（单属性上传多张图片）
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 具体业务对象Id（存具体关联对象Id，例如客户Id、项目Id）
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 具体实体Id（存具体业务表Id,例如项目的跟进记录Id、合同记录Id）
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// 文件夹所属Id
        /// </summary>
        public Guid? FileTypeId { get; set; }
    }
}
