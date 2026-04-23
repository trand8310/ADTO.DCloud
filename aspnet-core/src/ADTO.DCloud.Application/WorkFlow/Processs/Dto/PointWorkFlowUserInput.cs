using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 指派流程审批人
    /// </summary>
    public class PointWorkFlowUserInput
    {

        /// <summary>
        /// 流程任务主键
        /// </summary>
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 指派人员集合
        /// </summary>
        public List<WFProcessPoint> PointList { get; set; }
    }
    /// <summary>
    /// 指派人员
    /// </summary>
    public class WFProcessPoint
    {
        /// <summary>
        /// 流程节点
        /// </summary>
        public Guid UnitId { get; set; }
        /// <summary>
        /// 指派的审批人
        /// </summary>
        public string UserIds { get; set; }
    }
}
