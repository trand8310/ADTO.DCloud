using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 获取自定义流程请求参数
    /// </summary>
    public class GetMyListInput
    {
        /// <summary>
        /// 流程分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
