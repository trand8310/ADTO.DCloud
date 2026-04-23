using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Tasks.Dto
{
    /// <summary>
    /// 获取等待任务请求参数
    /// </summary>
    public class GetAwaitTaskListInput
    {
        /// <summary>
        /// 流程实例Id
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 流程单元节点Id
        /// </summary>
        public Guid UnitId { get; set; }
    }
}
