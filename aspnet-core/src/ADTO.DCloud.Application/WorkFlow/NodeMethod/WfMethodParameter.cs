using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.NodeMethod
{
    /// <summary>
    /// 流程绑定方法参数
    /// </summary>
    public class WfMethodParameter
    {
        /// <summary>
        /// 流程进程Id
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 子流程进程主键
        /// </summary>
        public Guid ChildProcessId { get; set; }
        /// <summary>
        /// 当前任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 操作码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 当前操作用户
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 当前用户账号
        /// </summary>
        public string UserAccount { get; set; }
        /// <summary>
        /// 当前用户公司
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 当前用户部门
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string Des { get; set; }
    }
}
