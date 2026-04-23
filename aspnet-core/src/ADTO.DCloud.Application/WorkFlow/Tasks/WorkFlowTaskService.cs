using ADTO.DCloud.Authorization;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.Delegates;
using ADTO.DCloud.WorkFlow.Processes;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Schemes.Dto;
using ADTO.DCloud.WorkFlow.Tasks.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Expressions;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using NUglify.JavaScript.Syntax;
using Stripe;
using Stripe.Climate;
using Stripe.Tax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;

namespace ADTO.DCloud.WorkFlow.Tasks
{
    /// <summary>
    /// 流程任务
    /// </summary>
    [ADTOSharpAuthorize]
    public class WorkFlowTaskService : DCloudAppServiceBase, IWorkFlowTaskService
    {

        private readonly IRepository<WorkFlowTask, Guid> _taskRepository;
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly DataBaseService _dataBaseService;

        #region ctor
        public WorkFlowTaskService(IRepository<WorkFlowTask, Guid> taskRepository,
            DataBaseService dataBaseService,
            IDelegateAppservice delegateService,
            IRepository<WorkFlowProcess, Guid> processRepository)
        {
            _taskRepository = taskRepository;
            _dataBaseService = dataBaseService;
            _processRepository = processRepository;
        }
        #endregion


        #region Methods


        #region 获取等待任务列表（用于网关判断是否所有支路都执行完毕）
        /// <summary>
        /// 获取等待任务列表（用于网关判断是否所有支路都执行完毕）
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowTaskDto>> GetAwaitTaskList(GetAwaitTaskListInput input)
        {
            var query = await _taskRepository.GetAll().Where(q => q.ProcessId.Equals(input.ProcessId) && q.UnitId == input.UnitId && q.Type == 21 && q.State == 1).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(query);
        }
        #endregion

        /// <summary>
        /// 根据Id获取任务实体
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkFlowTaskDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await _taskRepository.GetAsync(input.Id);
            return ObjectMapper.Map<WorkFlowTaskDto>(entity);
        }

        /// <summary>
        /// 获取最近的完成任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowTaskDto>> GetLastFinishTaskList(Guid processId, Guid unitId)
        {
            var query = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId && t.Type == 1 && t.IsLast == 1 && t.State == 3).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(query);
        }
        /// <summary>
        /// 获取最近的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowTaskDto>> GetLastTaskList(Guid processId, Guid unitId)
        {
            var list = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId && t.IsLast == 1 && (t.Type == 1 || t.Type == 3 || t.Type == 4 || t.Type == 5 || t.Type == 6)).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(list);
        }
        /// <summary>
        /// 获取最近的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<WorkFlowTaskDto> GetLastTask(Guid processId, Guid unitId)
        {
            var task = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId && (t.Type == 1 || t.Type == 3 || t.Type == 4 || t.Type == 5 || t.Type == 6)).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowTaskDto>(task);
        }
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="type"></param>
        /// <param name="prevTaskId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowTaskDto>> GetTaskList(Guid processId, int type, string prevTaskId)
        {
            var list = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.Type == type && t.PrevTaskId == prevTaskId).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(list);
        }

        /// <summary>
        /// 获取流程任务实体（根据来源任务）
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<WorkFlowTaskDto> GetEntityPrevTaskId(string id)
        {
            var entity = await _taskRepository.GetAll().Where(t => t.PrevTaskId == id).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowTaskDto>(entity);
        }
        /// <summary>
        /// 获取流程任务通过节点Id
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<WorkFlowTaskDto> GetEntityByUnitId(Guid processId, Guid unitId)
        {
            var entity = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowTaskDto>(entity);
        }
        /// <summary>
        /// 获取最近的不是驳回产生的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<WorkFlowTaskDto> GetLastNotRejectTask(Guid processId, Guid unitId)
        {
            var task = await _taskRepository.GetAll().Where(t =>
                t.ProcessId == processId
                && t.UnitId == unitId
                && t.IsReject != 1
                && (t.Type == 1 || t.Type == 3 || t.Type == 5)).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowTaskDto>(task);
        }
        /// <summary>
        /// 获取最近的驳回产生的任务
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<WorkFlowTaskDto> GetLastRejectTask(Guid processId, Guid unitId)
        {
            var task = await _taskRepository.GetAll().Where(t =>
               t.ProcessId == processId
                && t.UnitId == unitId
                && t.IsReject == 1
                && (t.Type == 1 || t.Type == 3 || t.Type == 5)).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowTaskDto>(task);
        }

        /// <summary>
        /// 获取加签任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowTaskDto>> GetSignTaskList(Guid taskId)
        {
            var list = await _taskRepository.GetAll().Where(q => q.Type == 6 && q.FirstId == taskId).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(list);
        }
        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="wfTaskEntity">任务日志</param>
        public async System.Threading.Tasks.Task Add(WorkFlowTaskDto input)
        {
            var entity = ObjectMapper.Map<WorkFlowTask>(input);
            var user = UserManager.GetUser(ADTOSharpSession.ToUserIdentifier());
            entity.CreatorUserName = user.Name;
            entity.IsLast = 1;
            await _taskRepository.InsertAsync(entity);
        }
        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="wfTaskEntity">任务日志</param>
        public async System.Threading.Tasks.Task UpdateTask(WorkFlowTaskDto input)
        {
            var entity = await _taskRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, entity);
            var user = UserManager.GetUser(ADTOSharpSession.ToUserIdentifier());
            entity.CreatorUserName = user.Name;
            await _taskRepository.UpdateAsync(entity);
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async System.Threading.Tasks.Task UpdateLast(Guid processId, Guid unitId)
        {
            var task = await _taskRepository.GetAll().Where(q => q.ProcessId.Equals(processId) && q.UnitId.Equals(unitId)).FirstOrDefaultAsync();
            if (task == null || task.Id == Guid.Empty)
                throw new UserFriendlyException("更新失败,任务不存在！");
            task.IsLast = 0;
            await _taskRepository.UpdateAsync(task);
        }
        /// <summary>
        /// 修改流程任务
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WorkFlowTaskDto> UpdateAsync(WorkFlowTaskDto input)
        {
            var task = await _taskRepository.GetAsync(input.Id);
            ObjectMapper.Map(input, task);
            await _taskRepository.UpdateAsync(task);
            return input;
        }
        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <param name="unitId">单元节点id</param>
        /// <param name="type">任务类型</param>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        [HiddenApi]
        public async System.Threading.Tasks.Task CloseTask(Guid processId, Guid unitId, int type, Guid? taskId = null)
        {
            var task = await _taskRepository.GetAll().Where(q => q.ProcessId.Equals(processId) && q.UnitId == unitId && q.Type == type).FirstOrDefaultAsync();
            if (task == null || task.Id == Guid.Empty)
                throw new UserFriendlyException("关闭失败,任务不存在！");
            task.State = 4;
            task.UpdateTaskId = taskId;
            await _taskRepository.UpdateAsync(task);
        }


        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="processId">流程主键</param>
        /// <param name="token">任务令牌</param>
        /// <param name="taskId">任务id（除去该任务不关闭）</param>
        /// <returns></returns>
        [HiddenApi]
        public async System.Threading.Tasks.Task CloseTask3(Guid processId, string token, Guid? taskId)
        {
            var expression = _taskRepository.GetAll();
            expression = expression.Where(t => t.ProcessId == processId && t.Token == token && t.State != 2 && t.State != 3 && t.State != 4 && t.State != 6 && t.State != 7 && t.State != 9);
            if (!taskId.IsEmpty())
            {
                expression = expression.Where(t => t.Id != taskId);
            }
            var list = await expression.ToListAsync();
            foreach (var item in list)
            {
                item.State = 4;
                item.UserId = ADTOSharpSession.GetUserId();
                await _taskRepository.UpdateAsync(item);
            }
        }

        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="processId">流程主键</param>
        /// <returns></returns>
        [HiddenApi]
        public async System.Threading.Tasks.Task CloseTaskByProcessId(Guid processId)
        {
            var task = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.State != 2 && t.State != 3 && t.State != 4 && t.State != 6 && t.State != 7 && t.State != 9).FirstOrDefaultAsync();
            task.State = 4;
            await _taskRepository.UpdateAsync(task);
        }
   
        /// <summary>
        /// 获取节点任务的最大人数
        /// </summary>
        /// <param name="processId">流程实例Id</param>
        /// <param name="unitId">流程单元节点Id</param>
        /// <returns></returns>
        public async Task<int> GetTaskUserMaxNum(Guid processId, Guid unitId)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                StringBuilder strSql = new StringBuilder();
                strSql.Append(" select COUNT(1) from  WorkFlowTasks t where ProcessId = @processId AND UnitId =@unitId  AND (Type = 1 OR Type = 3 OR Type = 5)   group by Token order by COUNT(1) ");
                parameters.Add("processId", processId);
                parameters.Add("unitId", unitId);
                var data = await _dataBaseService.FindTable(strSql.ToString(), parameters);

                if (data.Rows.Count > 0)
                {
                    return int.Parse(data.Rows[0][0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 打开任务
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <param name="unitId">单元节点id</param>
        /// <param name="type">任务类型</param>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task OpenTask(Guid processId, Guid unitId, int type, Guid taskId)
        {
            var list = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.UnitId == unitId && t.Type == type).ToListAsync();
            foreach (var item in list)
            {
                item.State = 1;
                item.UpdateTaskId = taskId;
                await _taskRepository.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 更新任务读写状态
        /// </summary>
        /// <param name="taskId"></param>
        public async System.Threading.Tasks.Task UpdateTaskReadStatus(Guid taskId)
        {
             await this._taskRepository.UpdateAsync(taskId, async entity =>
            {
                entity.ReadStatus = 1;
                entity.ReadTime = DateTime.Now;
            });
            //var entity = await _taskRepository.GetAsync(taskId);
            //entity.ReadStatus = 1;
            //entity.ReadTime = DateTime.Now;
            //await _taskRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteTaskByProcessId(Guid processId)
        {
            await _taskRepository.DeleteAsync(t => t.ProcessId == processId);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <param name="prevTaskId">来源任务密钥</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteTaskByProIdAndTskId(Guid processId, string prevTaskId)
        {
            await _taskRepository.DeleteAsync(t => t.ProcessId == processId && t.PrevTaskId == prevTaskId);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="id">任务主键</param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task DeleteByFirstId(Guid processId)
        {
            await _taskRepository.DeleteAsync(t => t.FirstId == processId);
        }
        #endregion

        /// <summary>
        /// 获取未完成的任务
        /// </summary>
        /// <param name="processId">流程进程主键</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowTaskDto>> GetUnFinishTaskList(Guid processId)
        {
            var list = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && ((t.State == 1 && (t.Type == 1 || t.Type == 2 || t.Type == 3 || t.Type == 4 || t.Type == 5 || t.Type == 6 || t.Type == 7)) || (t.State == 8 && t.Type == 3))).OrderByDescending(d => d.CreationTime).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(list);
        }

        /// <summary>
        /// 获取未完成的任务
        /// </summary>
        /// <param name="processIds">流程进程主键</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowTaskDto>> GetUnFinishTaskListByProIds(List<Guid> processIds)
        {
            var list = await _taskRepository.GetAll().Where(t => processIds.Contains(t.ProcessId) && ((t.State == 1 && (t.Type == 1 || t.Type == 2 || t.Type == 3 || t.Type == 4 || t.Type == 5 || t.Type == 6 || t.Type == 7)) || (t.State == 8 && t.Type == 3))).OrderByDescending(d => d.CreationTime).ToListAsync();
            return ObjectMapper.Map<List<WorkFlowTaskDto>>(list);
        }
        /// <summary>
        /// 获取沟通任务(未完成的)
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<IEnumerable<WorkFlowTask>> GetUnFinishedConnectTaskList(Guid taskId)
        {
            var list = await _taskRepository.GetAll().Where(t => t.Type == 7 && t.FirstId == taskId && (t.State == 1 || t.State == 9)).ToListAsync();
            return list;
        }
        /// <summary>
        /// 作废任务
        /// </summary>
        /// <param name="processId">流程进程</param>
        /// <returns></returns>
        [HiddenApi]
        public async System.Threading.Tasks.Task VirtualDelete(Guid processId)
        {
            var list = await _taskRepository.GetAll().Where(t => t.ProcessId == processId && t.State == 1).ToListAsync();
            foreach (var item in list)
            {
                item.State = 7;
                await _taskRepository.UpdateAsync(item);
            }
        }
        /// <summary>
        /// 获取流程任务实体
        /// </summary>
        /// <param name="token">密钥</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<WorkFlowTaskDto> GetEntityByToken(string token, Guid userId)
        {
            var entity = await _taskRepository.FirstOrDefaultAsync(t => t.Token == token && t.UserId == userId && t.State == 1);
            return ObjectMapper.Map<WorkFlowTaskDto>(entity);
        } 
    }
}

