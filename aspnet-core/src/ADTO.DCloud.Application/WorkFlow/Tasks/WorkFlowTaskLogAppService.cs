using ADTO.DCloud.Authorization;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.Tasks.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;


using ADTOSharp.Linq.Extensions;



namespace ADTO.DCloud.WorkFlow.Tasks
{
    /// <summary>
    /// 流程日志
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_WorkFlowTaskLog)]
    public class WorkFlowTaskLogAppService : DCloudAppServiceBase, IWorkFlowTaskLogAppService
    {
        private readonly IRepository<WorkFlowTaskLog, Guid> _taskLogRepository;
        public WorkFlowTaskLogAppService(IRepository<WorkFlowTaskLog, Guid> taskLogRepository)
        {
            _taskLogRepository = taskLogRepository;

        }
        /// <summary>
        /// 获取流程进程的任务处理日志
        /// </summary>
        /// <param name="processId">流程进程主键</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowTaskLogDto>> GetLogList(Guid processId)
        {
            var list = await _taskLogRepository.GetAll().Where(t => t.ProcessId.Equals(processId)).OrderByDescending(d => d.CreationTime).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskLogDto>>(list);
        }
        /// <summary>
        /// 获取任务日志列表
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowTaskLogDto>> GetLogList1(Guid processId, Guid unitId)
        {
            var list = await _taskLogRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskLogDto>>(list);
        }

        /// <summary>
        /// 获取流程进程的任务处理日志
        /// </summary>
        /// <param name="processId">流程进程主键</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowTaskLogDto>> GetLogListByProcessId(Guid processId)
        {
            var list = await _taskLogRepository.GetAll().Where(t => t.ProcessId == processId).OrderByDescending(d => d.CreationTime).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskLogDto>>(list);
        }

        /// <summary>
        /// 获取任务日志列表
        /// </summary>
        /// <param name="taskId">任务Id</param>
        /// <returns></returns>
        public async Task<WorkFlowTaskLogDto> GetLogEntity(EntityDto<Guid> input)
        {
            var log = await _taskLogRepository.GetAll().Where(d => d.TaskId.Equals(input.Id)).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowTaskLogDto>(log);
        }

        /// <summary>
        /// 保存任务日志
        /// </summary>
        /// <param name="wfTaskLogEntity">任务日志</param>
        public async System.Threading.Tasks.Task AddLog(WorkFlowTaskLog wfTaskLogEntity)
        {
            var user = UserManager.GetUser(ADTOSharpSession.ToUserIdentifier());
            wfTaskLogEntity.UserId = ADTOSharpSession.GetUserId();
            wfTaskLogEntity.UserName = user.Name;
            await _taskLogRepository.InsertAsync(wfTaskLogEntity);
        }
        /// <summary>
        /// 更新任务日志状态为不是最近处理的
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task UpdateLog(Guid processId, Guid unitId)
        {
            var entitys = await _taskLogRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId && t.IsLast == 1 && t.UserId == ADTOSharpSession.GetUserId()).ToListAsync();
            foreach (var entity in entitys)
            {
                entity.IsLast = 0;
                await _taskLogRepository.UpdateAsync(entity);
            }
        }
        /// <summary>
        /// 关闭任务日志撤销
        /// </summary>
        /// <param name="processId">流程主键</param>
        /// <param name="taskId">任务主键</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task CloseTaskLogCancel(Guid processId, string taskId)
        {
            var list = await _taskLogRepository.GetAll()
                .WhereIf(taskId == "create", x => x.ProcessId == processId && x.OperationCode == "create")
                .WhereIf(taskId != "create", x => x.ProcessId == processId && x.TaskId.Equals(Guid.Parse(taskId)))
                .ToListAsync();
            foreach (var item in list)
            {
                item.IsCancel = 0;
                await _taskLogRepository.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 关闭任务日志撤销
        /// </summary>
        /// <param name="taskId">流程任务Id</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task CloseTaskLogCancelByTaskId(Guid taskId)
        {
            var entitys = await _taskLogRepository.GetAll().Where(t => t.TaskId == taskId).ToListAsync();
            foreach (var item in entitys)
            {
                item.IsCancel = 0;
                await _taskLogRepository.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 删除任务日志
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteLogByProcessId(Guid processId)
        {
            await _taskLogRepository.DeleteAsync(t => t.ProcessId.Equals(processId));
        }

        /// <summary>
        /// 删除任务日志
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <param name="taskId">来源任务ID</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteLogByProIdAndTskId(Guid processId, Guid taskId)
        {
            await _taskLogRepository.DeleteAsync(t => t.ProcessId.Equals(processId) && t.TaskId == taskId && t.IsLast == 1);
        }

        /// <summary>
        /// 删除系统生成任务日志
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <param name="preUnitId">来源id</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteSystemLog(Guid processId, Guid preUnitId)
        {
            var log = await _taskLogRepository.GetAll().Where(t => t.ProcessId == processId && t.PrevUnitId == preUnitId &&
                (t.TaskType == 24 || t.TaskType == 25 || t.TaskType == 27)).OrderByDescending(o => o.CreationTime).FirstOrDefaultAsync();
            if (log != null && !log.Id.IsEmpty())
            {
                await _taskLogRepository.DeleteAsync(t => t.Id == log.Id);
            }
        }


        /// <summary>
        /// 删除任务日志
        /// </summary>
        /// <param name="taskId">任务主键</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task DeleteLogByTaskId(Guid taskId)
        {
            await _taskLogRepository.DeleteAsync(t => t.TaskId.Equals(taskId));
        }
        /// <summary>
        /// 修改审批意见---process/taskdes/id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="des"></param>
        [HiddenApi]
        public async Task<JsonResult> UpdateWorkFlowLogRemark(Guid id, string des)
        {
            try
            {
                var wfTaskLogEntity = await _taskLogRepository.GetAsync(id);
                wfTaskLogEntity.Remark = des;
                await _taskLogRepository.UpdateAsync(wfTaskLogEntity);
                return new JsonResult(new
                {
                    Message = L("修改成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("修改失败：" + ex.Message);
            }
        }
    }
}

