using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 转办审批人
    /// </summary>
    public class TransferUserInput
    {

        /// <summary>
        /// 流程任务主键
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 转办人员
        /// </summary>
        public Guid ToUserId { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Des { get; set; }
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
