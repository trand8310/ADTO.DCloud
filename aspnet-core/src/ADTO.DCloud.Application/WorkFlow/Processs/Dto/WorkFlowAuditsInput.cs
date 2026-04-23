using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 批量审核请求参数
    /// </summary>
    public class WorkFlowAuditsInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 任务Id集合
        /// </summary>
        public string Ids { get; set; }
    }
}
