using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程参数
    /// </summary>
    public class WFParams
    {
        /// <summary>
        /// 是否已经有流程实例
        /// </summary>
        public bool HasInstance { get; set; }
        /// <summary>
        /// 是否是子流程
        /// </summary>
        public bool IsChild { get; set; }
        /// <summary>
        /// 父级流程任务主键
        /// </summary>
        public Guid? ParentTaskId { get; set; }
        /// <summary>
        /// 父级流程实例主键
        /// </summary>
        public Guid? ParentProcessId { get; set; }
        /// <summary>
        /// 流程模板
        /// </summary>
        public string Scheme { get; set; }
        /// <summary>
        /// 流程模板名称
        /// </summary>
        public string SchemeName { get; set; }
        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string SchemeCode { get; set; }
        /// <summary>
        /// 流程模板主键
        /// </summary>
        public Guid SchemeId { get; set; }
        /// <summary>
        /// 流程实例Id
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 审批人信息
        /// </summary>
        public string Auditers { get; set; }
        /// <summary>
        /// 流程发起用户
        /// </summary>
        public WFUserInfo CreateUser { get; set; }
        /// <summary>
        /// 当前执行用户
        /// </summary>
        public WFUserInfo CurrentUser { get; set; }
        /// <summary>
        /// 流程状态 0 默认运行状态 1 重新发起 2 运行结束 3草稿 4 作废
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 流程是否开始审批 1 是 0 不是
        /// </summary>
        public int IsStart { get; set; }

        /// <summary>
        /// 流程是否结束 1 是 0 不是
        /// </summary>
        public int IsFinished { get; set; }
        /// <summary>
        /// 下一个节点审批人
        /// </summary>
        public Dictionary<Guid, string> NextUsers { get; set; }
    }
}
