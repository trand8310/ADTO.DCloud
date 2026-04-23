using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    public class ShareWorkFlowInput
    {
        /// <summary>
        /// 流程任务主键
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 传阅人员,支持多人用逗号隔开
        /// </summary>
        public string ToUserId { get; set; }
    }
}
