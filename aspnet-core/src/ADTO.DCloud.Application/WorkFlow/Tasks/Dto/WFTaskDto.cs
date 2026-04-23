using ADTO.DCloud.WorkFlow.Processes;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Schemes;
using ADTO.DCloud.WorkFlow.Schemes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Tasks.Dto
{
    public class WFTaskDto
    {
        /// <summary>
        /// 当前任务
        /// </summary>
        public WorkFlowTaskDto Task { get; set; }
        /// <summary>
        /// 流程进程
        /// </summary>
        public WorkFlowProcessDto Process { get; set; }
        /// <summary>
        /// 流程模板信息
        /// </summary>
        public WorkFlowSchemeDto Scheme { get; set; }
        /// <summary>
        /// 审批日志
        /// </summary>
        public IEnumerable<WorkFlowTaskLogDto> Logs { get; set; }
        /// <summary>
        /// 任务列表
        /// </summary>
        public IEnumerable<WorkFlowTaskDto> Tasks { get; set; }
    }
}
