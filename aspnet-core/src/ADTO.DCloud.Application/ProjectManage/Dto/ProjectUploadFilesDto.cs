using System;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目跟进记录表单附件对象
    /// </summary>
    public class ProjectUploadFilesDto
    {
        /// <summary>
        /// 图片Id（历史图片）
        /// </summary>
        public Guid? FileId { get; set; }

        /// <summary>
        /// 新上传的文件
        /// </summary>
        public string FileToken { get; set; }

        /// <summary>
        /// 编辑时填充图片
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }
    }
}
