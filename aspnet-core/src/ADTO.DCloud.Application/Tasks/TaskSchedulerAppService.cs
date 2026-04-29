using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using ADTOSharp.UI;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Tasks.Dto;
using ADTO.DCloud.DataIcons.Dto;
using ADTOSharp.Domain.Uow;
using System.Transactions;
using ADTO.DCloud.Tasks.TaskManage;

namespace ADTO.DCloud.Tasks;

/// <summary>
/// 任务管理管理
/// </summary>
public class TaskSchedulerAppService : DCloudAppServiceBase, ITaskSchedulerAppService
{
    #region Fields
    private readonly IRepository<TaskScheduler, Guid> _taskSchedulerRepository;
    private readonly IRepository<TaskExecutionHistory, Guid> _taskExecutionHistoryRepository;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IDynamicTaskManager _dynamicTaskManager;
    #endregion

    #region Ctor
    public TaskSchedulerAppService(
       IRepository<TaskScheduler, Guid> taskSchedulerRepository
       , IRepository<TaskExecutionHistory, Guid> taskExecutionHistoryRepository
       ,IRepository<User, Guid> userRepository
       ,IDynamicTaskManager dynamicTaskManager)
    {
        _taskSchedulerRepository = taskSchedulerRepository;
        _taskExecutionHistoryRepository = taskExecutionHistoryRepository;
        _userRepository = userRepository;
        _dynamicTaskManager = dynamicTaskManager;
    }
    #endregion

    #region Methods

    /// <summary>
    /// 添加系统任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task CreateTaskSchedulerAsync(CreateTaskSchedulerDto input)
    {
        var item = ObjectMapper.Map<TaskScheduler>(input);
        await _taskSchedulerRepository.InsertAsync(item);
        await _dynamicTaskManager.UpdateTaskAsync(ObjectMapper.Map<TaskSchedulerDto>(item));
    }

    /// <summary>
    /// 修改系统任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateTaskSchedulerAsync(CreateTaskSchedulerDto input)
    {
        var existInfo = await this._taskSchedulerRepository.GetAll().Where(p => p.Id == input.Id).FirstOrDefaultAsync();
        if (existInfo == null)
        {
            throw new UserFriendlyException("保存失败,任务不存在！");
        }
        var info = this._taskSchedulerRepository.Get(input.Id.Value);
        ObjectMapper.Map(input, info);

        //转换一下，否则其它字段也会置空
        await _taskSchedulerRepository.UpdateAsync(info);
        await _dynamicTaskManager.UpdateTaskAsync(ObjectMapper.Map<TaskSchedulerDto>(info));
    }

    /// <summary>
    /// 获取系统任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<TaskSchedulerDto> GetTaskSchedulerByIdAsync(EntityDto<Guid> input)
    {
        var info = await _taskSchedulerRepository.GetAsync(input.Id);
        return ObjectMapper.Map<TaskSchedulerDto>(info);
    }

    /// <summary>
    /// 删除系统任务
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>

    public async Task DeleteTaskSchedulerAsync(EntityDto<Guid> input)
    {
        await _taskSchedulerRepository.DeleteAsync(input.Id);
        await _dynamicTaskManager.RemoveTaskAsync(input.Id);
    }

    /// <summary>
    /// 系统任务列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<TaskSchedulerDto>> GetTaskSchedulerList()
    {
        var list = await this._taskSchedulerRepository.GetAll().OrderBy(p => p.CreationTime).ToListAsync();
        return ObjectMapper.Map<List<TaskSchedulerDto>>(list);
    }

    /// <summary>
    /// 获取启用状态的任务列表
    /// </summary>
    /// <returns></returns>
    [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
    public async Task<List<TaskSchedulerDto>> GetTaskSchedulerListByState()
    {
       var list= await this._taskSchedulerRepository.GetAll().Where(p => p.State).ToListAsync();
        return ObjectMapper.Map<List<TaskSchedulerDto>>(list);
    }

    /// <summary>
    /// 修改下次执行时间
    /// </summary>
    /// <returns></returns>
    [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
    public async Task UpdateNextExecutionTime(Guid Id,DateTime NextExecutionTime)
    {
        var list = await this._taskSchedulerRepository.GetAll().Where(p => p.State).ToListAsync();
        await this._taskSchedulerRepository.UpdateAsync(Id, async entity => { entity.NextExecutionTime = NextExecutionTime; });
    }

    /// <summary>
    /// 修改最后执行时间
    /// </summary>
    /// <returns></returns>
    [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
    public async Task UpdateLastExecutionTime(Guid Id, DateTime LastExecutionTime)
    {
        var list = await this._taskSchedulerRepository.GetAll().Where(p => p.State).ToListAsync();
        await this._taskSchedulerRepository.UpdateAsync(Id, async entity => { entity.LastExecutionTime = LastExecutionTime; });
    }
    /// <summary>
    /// 添加系统任务历史记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
    public async Task CreateTaskExecutionHistoryAsync(TaskExecutionHistoryDto input)
    {
        var info = ObjectMapper.Map<TaskExecutionHistory>(input);
        await _taskExecutionHistoryRepository.InsertAsync(info);
    }

    /// <summary>
    /// 获取指定任务
    /// </summary>
    /// <returns></returns>
    [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
    public async Task<TaskSchedulerDto> GetTaskSchedulerByIdUnitOfWork(Guid Id)
    {
        var info = await _taskSchedulerRepository.GetAsync(Id);
        return ObjectMapper.Map<TaskSchedulerDto>(info);
    }

    /// <summary>
    /// 系统任务分页接口
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<TaskSchedulerDto>> GetTaskSchedulerPageList(PagedDataIconsResultRequestDto input)
    {
        var query = this._taskSchedulerRepository.GetAll()
       .WhereIf(!string.IsNullOrWhiteSpace(input.KeyWord), p => p.Title.Contains(input.KeyWord));

        var totalCount = await query.CountAsync();
        var results = await query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToListAsync();
        return new PagedResultDto<TaskSchedulerDto>(totalCount, ObjectMapper.Map<List<TaskSchedulerDto>>(results));
    }
    #endregion
}

