using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Media.FileManage.Aliyun.Dto
{
    /// <summary>
    /// 上传成功后返回参数
    /// </summary>
    public class AliyunPutObjectDto
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string FileUrl { get; set; }

        public long ContentLength { get; set; }


    }
}
