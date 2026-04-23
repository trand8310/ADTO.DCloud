using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 沟通
    /// </summary>
    public class ConnectWorkFlowInput
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 1可以继续沟通，2不可以
        /// </summary>
        public int? ConnectType { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Des { get; set; }
        /// <summary>
        /// 加签人员
        /// </summary>
        public string UserIds { get; set; }
        /// <summary>
        /// 审批附件ID
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 审批要点
        /// </summary>
        public string Tag { get; set; }
    }
}
