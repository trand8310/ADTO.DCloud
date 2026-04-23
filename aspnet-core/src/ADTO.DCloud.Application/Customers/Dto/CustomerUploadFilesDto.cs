using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Customers.Dto
{
    /// <summary>
    /// 客户表单附件对象
    /// </summary>
    public class CustomerUploadFilesDto
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
