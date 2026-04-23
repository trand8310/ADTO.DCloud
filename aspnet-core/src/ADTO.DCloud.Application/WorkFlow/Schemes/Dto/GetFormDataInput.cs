using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 根据流程编码获取流程信息
    /// </summary>
    public class GetFormDataInput
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        public string Code { get; set; }
    }
}
