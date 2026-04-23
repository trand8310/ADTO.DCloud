using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 创建流程
    /// </summary>
    public class WorkFlowCreateInput
    {
        /// <summary>
        /// 流程进程实例
        /// </summary>
        public Guid ProcessId { get; set; }

        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string SchemeCode { get; set; }

        /// <summary>
        /// 流程发起人ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 下一节点审批人
        /// </summary>
        public Dictionary<Guid, string> NextUsers { get; set; }
        /// <summary>
        /// 关联流程ID
        /// </summary>
        public Guid? RProcessId { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Des { get; set; }
        /// <summary>
        /// 审批附件ID
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 流程等级
        /// </summary>
        public int? Level { get; set; }
        /// <summary>
        /// 流程密级
        /// </summary>
        public int? SecretLevel { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 表单数据
        /// </summary>
        public string FormData { get; set; }
    }
}
