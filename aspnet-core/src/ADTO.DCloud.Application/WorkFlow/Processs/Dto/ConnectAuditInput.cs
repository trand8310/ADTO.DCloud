using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 沟通审批
    /// </summary>
    public class ConnectAuditInput
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Des { get; set; }
        /// <summary>
        /// 审批操作码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 审批操作名称
        /// </summary>
        public string Name { get; set; }
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
