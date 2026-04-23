using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 获取下一步审核人请求参数
    /// </summary>
    public class GetNextAuditorsInput
    {
        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 流程进程主键
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 流程模板主键
        /// </summary>
        public Guid SchemeId { get; set; }
        /// <summary>
        /// 流程节点Id
        /// </summary>
        public Guid NodeId { get; set; }
        /// <summary>
        /// 流程操作代码
        /// </summary>
        public string OperationCode { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 下一审批节点
        /// </summary>
        public string NextNodeId { get; set; }

    }
}
