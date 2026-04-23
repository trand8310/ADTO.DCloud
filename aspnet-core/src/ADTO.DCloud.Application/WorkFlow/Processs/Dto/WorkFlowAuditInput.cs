using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程审核请求参数
    /// </summary>
    public class WorkFlowAuditInput
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
        /// 下一节点审批人
        /// </summary>
        public Dictionary<Guid, string> NextUsers { get; set; }
        /// <summary>
        /// 盖章ID
        /// </summary>
        public string StampImg { get; set; }
        /// <summary>
        /// 盖章密码
        /// </summary>
        public string StampPassWord { get; set; }
        /// <summary>
        /// 下一个审批节点
        /// </summary>
        public Guid NextId { get; set; }
        /// <summary>
        /// 审批附件ID
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 审批要点
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 驳回审批是否原路返回 1 是 其它不是
        /// </summary>
        public int? IsRejectBackOld { get; set; }

        /// <summary>
        /// 表单数据
        /// </summary>
        public string FormData { get; set; }
    }
}
