using ADTO.DCloud.WorkFlow.Tasks.Dto;
using ADTOSharp.Application.Services;
using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Tasks
{
    /// <summary>
    /// 流程任务
    /// </summary>
    public interface IWorkFlowTaskService : IApplicationService
    {
        /// <summary>
        /// 获取等待任务列表（用于网关判断是否所有支路都执行完毕）
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public Task<IEnumerable<WorkFlowTaskDto>> GetAwaitTaskList(GetAwaitTaskListInput input);

        /// <summary>
        /// 获取最近的完成任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public  Task<IEnumerable<WorkFlowTaskDto>> GetLastFinishTaskList(Guid processId, Guid unitId);
        /// <summary>
        /// 获取最近的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public  Task<IEnumerable<WorkFlowTaskDto>> GetLastTaskList(Guid processId, Guid unitId);
        /// <summary>
        /// 获取最近的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public Task<WorkFlowTaskDto> GetLastTask(Guid processId, Guid unitId);
        /// <summary>
        /// 获取最近的不是驳回产生的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public Task<WorkFlowTaskDto> GetLastNotRejectTask(Guid processId, Guid unitId);
        /// <summary>
        /// 获取最近的驳回产生的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public Task<WorkFlowTaskDto> GetLastRejectTask(Guid processId, Guid unitId);
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public Task UpdateLast(Guid processId, Guid unitId);
    }
}
