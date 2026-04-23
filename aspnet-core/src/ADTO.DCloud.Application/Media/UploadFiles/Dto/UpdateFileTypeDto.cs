using System;
using System.Collections.Generic;
 
namespace ADTO.DCloud.Media.UploadFiles.Dto
{
    /// <summary>
    /// 批量修改图片所属类别
    /// </summary>
    public class UpdateFileTypeIdDto
    {
        /// <summary>
        /// 文件Id 数组
        /// </summary>
        public List<Guid> FileIds { get; set; } = null!;

        /// <summary>
        /// 文件类型 FileType表Id
        /// </summary>
        public Guid FileTypeId { get; set; }
    }
}
