using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{

    /// <summary>
    /// 流程进程参数
    /// </summary>
    public class WFProcessDto
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 流程进程实例
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 父流程任务id
        /// </summary>
        public Guid ParentTaskId { get; set; }
        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string SchemeCode { get; set; }

        /// <summary>
        /// 流程模板版本id
        /// </summary>
        public Guid SchemeId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 流程发起人ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 流程转办审批人
        /// </summary>
        public Guid ToUserId { get; set; }
        /// <summary>
        /// 下一节点审批人
        /// </summary>
        public Dictionary<Guid, string> NextUsers { get; set; }
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
        /// 盖章ID
        /// </summary>
        public string StampImg { get; set; }
        /// <summary>
        /// 盖章密码
        /// </summary>
        public string StampPassWord { get; set; }
        /// <summary>
        /// 审批附件ID
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// 审批要点
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 下一个审批节点
        /// </summary>
        public Guid NextId { get; set; }
        /// <summary>
        /// 流程等级
        /// </summary>
        public int? Level { get; set; }
        /// <summary>
        /// 流程密级
        /// </summary>
        public int? SecretLevel { get; set; }
        /// <summary>
        /// 关联流程ID
        /// </summary>
        public Guid RProcessId { get; set; }

        /// <summary>
        /// 驳回审批是否原路返回 1 是 其它不是
        /// </summary>
        public int? IsRejectBackOld { get; set; }
        /// <summary>
        /// 加签策略(1串行，2并行)
        /// </summary>
        public int? SignType { get; set; }
        /// <summary>
        /// 沟通策略:1可以继续沟通，2不可以
        /// </summary>
        public int? ConnectType { get; set; }
    }
}
