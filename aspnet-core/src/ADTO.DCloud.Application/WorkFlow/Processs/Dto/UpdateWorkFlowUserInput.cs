using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    public class UpdateWorkFlowUserInput
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string UserIds { get; set; }
    }
}
