using ADTO.DCloud.WorkFlow.Tasks.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Tasks
{
    /// <summary>
    /// 流程日志
    /// </summary>
    public interface IWorkFlowTaskLogAppService
    {
        /// <summary>
        /// 获取任务日志列表
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public Task<IEnumerable<WorkFlowTaskLogDto>> GetLogList1(Guid processId, Guid unitId);
    }
}
