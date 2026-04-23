using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 加签
    /// </summary>
    public class SignFlowInput
    {
        /// <summary>
        /// 流程转办审批人
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 流程转办审批人
        /// </summary>
        public string ToUserId { get; set; }
        /// <summary>
        /// 加签策略(1串行，2并行)
        /// </summary>
        public int? SignType { get; set; }
        /// <summary>
        /// 审批附件ID
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 审批要点
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Des { get; set; }
    }
}
