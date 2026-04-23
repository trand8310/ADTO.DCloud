using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Posts;
using ADTO.DCloud.Authorization.UserRelations;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Authorization.Users.Delegation;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Common;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.FormScheme;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Massages;
using ADTO.DCloud.Organizations;
using ADTO.DCloud.WorkFlow.Delegates;
using ADTO.DCloud.WorkFlow.NodeMethod;
using ADTO.DCloud.WorkFlow.Processes;
using ADTO.DCloud.WorkFlow.Processs.Config;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Schemes;
using ADTO.DCloud.WorkFlow.Schemes.Dto;
using ADTO.DCloud.WorkFlow.Stamps;
using ADTO.DCloud.WorkFlow.Tasks;
using ADTO.DCloud.WorkFlow.Tasks.Dto;
using ADTOSharp;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Linq.Expressions;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Threading.Extensions;
using ADTOSharp.UI;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace ADTO.DCloud.WorkFlow.Processs
{
    /// <summary>
    /// 流程进程管理
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Process)]
    [ADTOSharpAuthorize]
    public class ProcessAppService : DCloudAppServiceBase, IProcessAppService
    {
        #region ctor
        private readonly IRepository<WorkFlowProcess, Guid> _processRepository;
        private readonly IRepository<WorkFlowScheme, Guid> _schemeRepository;
        private readonly IRepository<WorkFlowTask, Guid> _taskRepository;
        private readonly IRepository<WorkFlowTaskLog, Guid> _taskLogRepository;
        private readonly IRepository<UserPost, Guid> _userPostRepository;
        private readonly IRepository<UserRole, Guid> _userRoleRepository;
        private readonly IRepository<User, Guid> _userRepository;
        private readonly IRepository<EmployeeInfo, Guid> _employeeRepository;
        private readonly WorkFlowTaskService _workFlowTaskSerivce;
        private readonly CommonLookupAppService _userAppService;

        private readonly PostAppService _postAppService;
        private readonly DataBaseService _dataBaseService;
        private readonly OrganizationUnitAppService _organizationUnitAppService;
        private readonly WorkFlowSchemeinfoAppService _workFlowSchemeAppService;
        private readonly WorkFlowTaskLogAppService _workFlowTaskLogAppService;
        private readonly MessageAppService _messageAppService;
        private readonly UserRelationAppService _userRelationAppService;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly ICacheManager _cacheManager;
        private readonly StampAppService _stampAppService;
        private readonly IDelegateAppservice _delegateService;
        private readonly FormSchemeAppService _formSchemeAppService;
        private readonly UserDelegationAppService _userDelegationAppService;
        private readonly InvokeMethodHelper _invokeMethodHelper;

        private readonly IServiceProvider _serviceProvider;
        private readonly IDapperSqlExecutor _sqlExecutor;
        private readonly IWorkFlowEngineAppService _flowEngineAppService;
        private readonly IGuidGenerator _guidGenerator;
        public ProcessAppService(IRepository<WorkFlowProcess, Guid> processRepository,
           IRepository<Schemes.WorkFlowScheme, Guid> schemeRepository,
           IRepository<User, Guid> userRepository,
            CommonLookupAppService userAppService,
           IRepository<WorkFlowTask, Guid> taskRepository,
           IRepository<WorkFlowTaskLog, Guid> taskLogRepository,
           WorkFlowTaskService workFlowTaskSerivce,
           IRepository<UserPost, Guid> userPostRepository,
           IRepository<UserRole, Guid> userRoleRepository,
           PostAppService postAppService, DataBaseService dataBaseService,
           OrganizationUnitAppService organizationUnitAppService, IRoleManagementConfig roleManagementConfig
            , WorkFlowSchemeinfoAppService workFlowSchemeinfoAppService,
           WorkFlowTaskLogAppService workFlowTaskLogAppService,
            MessageAppService messageAppService,
            UserRelationAppService userRelationAppService,
        ICacheManager cacheManager,
        StampAppService stampAppService,
        IDelegateAppservice delegateService,

        FormSchemeAppService formSchemeAppService,
        UserDelegationAppService userDelegationAppService,
        InvokeMethodHelper invokeMethodHelper,
        IServiceProvider serviceProvider,
        IDapperSqlExecutor sqlExecutor,
        IWorkFlowEngineAppService flowEngineAppService,
        IRepository<EmployeeInfo, Guid> employeeRepository,
            IGuidGenerator guidGenerator
            )
        {
            //HttpClientHelper httpClientHelper
            _processRepository = processRepository;
            _taskRepository = taskRepository;
            _workFlowTaskSerivce = workFlowTaskSerivce;
            _userPostRepository = userPostRepository;
            _userRoleRepository = userRoleRepository;
            _postAppService = postAppService;
            _dataBaseService = dataBaseService;
            _organizationUnitAppService = organizationUnitAppService;
            _roleManagementConfig = roleManagementConfig;
            _workFlowSchemeAppService = workFlowSchemeinfoAppService;
            _workFlowTaskLogAppService = workFlowTaskLogAppService;
            _userRepository = userRepository;
            _userAppService = userAppService;
            _messageAppService = messageAppService;
            _userRelationAppService = userRelationAppService;
            _cacheManager = cacheManager;
            _stampAppService = stampAppService;
            _schemeRepository = schemeRepository;
            _taskLogRepository = taskLogRepository;
            _delegateService = delegateService;
            _formSchemeAppService = formSchemeAppService;
            _userDelegationAppService = userDelegationAppService;
            _invokeMethodHelper = invokeMethodHelper;
            _serviceProvider = serviceProvider;
            _sqlExecutor = sqlExecutor;
            _flowEngineAppService = flowEngineAppService;
            _employeeRepository = employeeRepository;
            _guidGenerator = guidGenerator;
        }
        #endregion

        #region Methods
          /// <summary>
        /// 获取我的已办任务-数量
        /// </summary>
        /// <returns></returns>
        public async Task<HomeMyCompletedAndUncompletedDto> GetMyCompletedAndUnCompletedCount(GetMyCompletedAndUnCompletedInput input)
        {
            if (ADTOSharpSession.UserId == null)
                return new HomeMyCompletedAndUncompletedDto() { CompletedCount = 0, UnCompletedCount = 0 };
            //下面的方式是我的已办任务的写法优化版本
            var userId = ADTOSharpSession.GetUserId();
            // 1. 预定义集合
            var validStates = new[] { 3, 5, 6, 8 };
            var validTypes = new[] { 1, 3, 5, 6, 7 };
            // 3. 统一查询
            var query1 =
                from task in _taskRepository.GetAll()
                join log in _taskLogRepository.GetAll() on task.Id equals log.TaskId
                // 注意：这里直接用 Inner Join log，因为你的逻辑要求 log.IsLast == 1
                // 如果业务允许任务没有日志，这里才用 Left Join + DefaultIfEmpty
                where log.IsLast == 1
                join process in _processRepository.GetAll() on task.ProcessId equals process.Id into procGroup
                from process in procGroup.DefaultIfEmpty() // 默认左连接
                    // --- 核心过滤逻辑 ---
                where validStates.Contains(task.State ?? 0)
                where validTypes.Contains(task.Type ?? 0)
                where task.IsAuto == 0 || task.IsAuto == null
                where task.UserId == userId || log.UserId == userId
                select new
                {
                    task,
                    log,
                    process
                };
            query1 = query1
                // 动态条件：流程相关过滤
                // 技巧：如果传入了流程过滤条件，我们显式要求 process 不能为 null。
                // 这样既利用了 Left Join 的灵活性，又在有需求时退化为 Inner Join 的效果，且无需写两套代码。
                .WhereIf(!string.IsNullOrEmpty(input.FormKeyword), x =>
                    x.process != null && x.process.Keyword.Contains(input.FormKeyword))
                .WhereIf(!string.IsNullOrEmpty(input.SchemaName), x =>
                    x.process != null && x.process.SchemeName == input.SchemaName)
                   .WhereIf(!string.IsNullOrEmpty(input.UserId), x => x.process != null && x.process.CreatorUserId == Guid.Parse(input.UserId))
                // 时间范围
                .WhereIf(input.StartDate != null && input.EndDate != null, x =>
                    x.task.CreationTime >= input.StartDate && x.task.CreationTime <= input.EndDate);
            int count = query1.Count();   // 获取符合条件的记录总数

            return new HomeMyCompletedAndUncompletedDto() { CompletedCount = count, UnCompletedCount = 0 };
        }

        /// <summary>
        /// 获取我的已办任务-completed/mypage
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="searchParams">查询参数</param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowTaskDto>> GetMyCompletedPageList(GetMyUncompletedPageInput input)
        {
            var userId = ADTOSharpSession.GetUserId();
            var query = from task in _taskRepository.GetAll()
                        join t2 in _taskLogRepository.GetAll() on task.Id equals t2.TaskId into t2
                        from log in t2.DefaultIfEmpty()
                        join t3 in _processRepository.GetAll() on task.ProcessId equals t3.Id into t3
                        from process in t3.DefaultIfEmpty()
                        select new { CreationTime = task.CreationTime, log, task, process, ProcessId = task.ProcessId, IsUrge = task.IsUrge, IsUp = task.IsUp, IsUp2 = task.IsUp2 };
            //select new { CreationTime = task.CreationTime, log, task, process, ProcessId = task.ProcessId, IsUrge = task.IsUrge, IsUp = task.IsUp, IsUp2 = task.IsUp2 };
            query = query.Where(t => t.log.IsLast == 1)//是否最近一次处理 
                   .Where(t => t.task.State == 3 || t.task.State == 5 || t.task.State == 6 || t.task.State == 8)//任务状态
                   .Where(t => t.task.Type == 1 || t.task.Type == 3 || t.task.Type == 5 || t.task.Type == 6 || t.task.Type == 7)//审批任务
                   .Where(t => t.task.IsAuto == 0 || t.task.IsAuto == null)//是否自动执行
                   .Where(t => t.task.UserId == userId || t.log.UserId == userId)//任务执行人

                   .WhereIf(!string.IsNullOrEmpty(input.Keyword), t => t.task.ProcessTitle.Contains(input.Keyword) || t.task.UnitName.Contains(input.Keyword))
                   .WhereIf(!string.IsNullOrEmpty(input.Code), t => t.task.ProcessCode == input.Code)
                   .WhereIf(input.StartDate != null && input.EndDate != null, t => t.task.CreationTime >= input.StartDate && t.task.CreationTime <= input.EndDate)
                   .WhereIf(!string.IsNullOrEmpty(input.FormKeyword), t => t.process.Keyword.Contains(input.FormKeyword))
                   .WhereIf(!string.IsNullOrEmpty(input.SchemaName), t => t.process.SchemeName == input.SchemaName)
                   .WhereIf(!string.IsNullOrEmpty(input.UserId), t => t.process.CreatorUserId == Guid.Parse(input.UserId));
            if (!string.IsNullOrEmpty(input.Where))
            {
                string where = $"t.ProcessId in ({AESHelper.AesDecrypt(input.Where, "ADTODCloud")})";
                query = query.Where(where);
            }

            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();

            var processIds = list.Select(d => d.process.Id).ToList();
            var taskUnFinishList = await _workFlowTaskSerivce.GetUnFinishTaskListByProIds(processIds);
            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<WorkFlowTaskDto>(item.task);

                dto.IsCancel = item.log.IsCancel;
                dto.OperationName = item.log.OperationName;
                dto.MakeUserId = item.log.UserId.ToString();
                dto.MakeTime = item.log.CreationTime;

                dto.PorcessUserId = item.process.CreatorUserId;
                dto.IsFinished = item.process.IsFinished;
                dto.IsDelete = item.process.IsActive;
                dto.Level = item.process.Level;
                dto.SchemeName = item.process.SchemeName;

                var myTaskUnFinishList = taskUnFinishList.Where(t => t.ProcessId == item.task.ProcessId);
                var dic = new Dictionary<Guid, bool>();
                dto.Step = 100;
                foreach (var unFinishItem in myTaskUnFinishList)
                {
                    if (!dic.ContainsKey(unFinishItem.UnitId) && unFinishItem.Type != 2)
                    {
                        dic.Add(unFinishItem.UnitId, true);
                        if (!string.IsNullOrEmpty(dto.UnitNames))
                        {
                            dto.UnitNames += ",";
                        }
                        dto.UnitNames += unFinishItem.UnitName;
                        if (item.task.Step > unFinishItem.Step)
                        {
                            dto.Step = unFinishItem.Step;
                        }
                    }
                }

                return dto;
            }).ToList();
            return new PagedResultDto<WorkFlowTaskDto>(totalCount, result);
        }


        /// <summary>
        /// 获取我的待办任务-uncompleted/mypage
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="input">查询参数</param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowTaskDto>> GetMyUncompletedPageList(GetMyUncompletedPageInput input)
        {
            var currentUserId = ADTOSharpSession.UserId;

            var query = from task in _taskRepository.GetAll()
                        join t3 in _processRepository.GetAll() on task.ProcessId equals t3.Id into t3
                        from process in t3.DefaultIfEmpty()
                        select new { CreationTime = task.CreationTime, task, process, ProcessId = task.ProcessId, IsUrge = task.IsUrge, IsUp = task.IsUp };
            query = query.Where(t => t.task.State == 1 || t.task.State == 9)
             .Where(t => t.task.Type != 2)
             .WhereIf(input.UserId != "-1" || string.IsNullOrEmpty(input.UserId), t => t.task.UserId == currentUserId)//当前用户
             .WhereIf(input.UserId != "-1" && !string.IsNullOrEmpty(input.UserId), t => t.task.UserId == Guid.Parse(input.UserId))//任务执行人
             .WhereIf(!string.IsNullOrEmpty(input.Keyword), t => t.task.ProcessTitle.Contains(input.Keyword) || t.task.UnitName.Contains(input.Keyword))//根据关键词查询
             .WhereIf(!string.IsNullOrEmpty(input.Code), t => t.task.ProcessCode == input.Code)//根据流程编码查询
             .WhereIf(input.StartDate != null && input.EndDate != null, t => t.task.CreationTime >= input.StartDate && t.task.CreationTime <= input.EndDate)//根据任务创建时间查询
             .WhereIf(!string.IsNullOrEmpty(input.FormKeyword), t => t.process.Keyword.Contains(input.FormKeyword))//根据表单关键字
             .WhereIf(input.IsBatchAudit, t => t.task.IsBatchAudit == 1)//是否允许批量审批
             .WhereIf(!string.IsNullOrEmpty(input.SchemaName), t => t.process.SchemeName == input.SchemaName)//流程模板名称
             .WhereIf(!string.IsNullOrEmpty(input.UserId), t => t.process.CreatorUserId == Guid.Parse(input.UserId))//流程申请人
             ;

            if (!string.IsNullOrEmpty(input.Where))
            {
                string where = $"t.ProcessId in ({AESHelper.AesDecrypt(input.Where, "DCloudADTO")})";
                query = query.Where(where);
            }
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();

            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<WorkFlowTaskDto>(item.task);
                dto.CreationTime = item.process.CreationTime;//流程申请时间
                dto.CreatorUserId = item.process.CreatorUserId;//流程申请人
                dto.CreatorUserName = item.process.CreateUserName;//流程申请人
                dto.PorcessUserId = item.process.CreatorUserId;
                dto.IsFinished = item.process.IsFinished;
                dto.IsDelete = item.process.IsActive;
                dto.Level = item.process.Level;
                dto.SchemeName = item.process.SchemeName;
                return dto;
            }).ToList();
            return new PagedResultDto<WorkFlowTaskDto>(totalCount, result);
        }

        /// <summary>
        /// 获取流程信息 workflow/process/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<WFTaskDto> GetProcessAsync(EntityDto<Guid> input)
        {
            var res = new WFTaskDto();
            res.Process = ObjectMapper.Map<WorkFlowProcessDto>(await _processRepository.FirstOrDefaultAsync(input.Id));
            if (res.Process == null)
                return res;
            res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(await _schemeRepository.FirstOrDefaultAsync(res.Process.SchemeId ?? Guid.Empty));
            var logs = await _taskLogRepository.GetAll().Where(q => q.ProcessId.Equals(input.Id)).OrderByDescending(d => d.CreationTime).ToListAsync();
            res.Logs = ObjectMapper.Map<List<WorkFlowTaskLogDto>>(logs);
            res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(input.Id);
            return res;
        }
        /// <summary>
        /// 获取任务 workflow/process/draft/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<WFTaskDto> GetDraftAsync(Guid id)
        {
            var res = new WFTaskDto();
            res.Process = ObjectMapper.Map<WorkFlowProcessDto>(await _processRepository.FirstOrDefaultAsync(id));
            if (res.Process == null)
                return res;
            res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(await _schemeRepository.FirstOrDefaultAsync(res.Process.SchemeId ?? Guid.Empty));
            return res;
        }

        /// <summary>
        /// 获取我的待办任务(批量审批任务)-uncompleted/batchpage
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="searchParams">查询参数</param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowTaskDto>> GetMyUncompletedBatchPageList(GetMyUncompletedPageInput searchParams)
        {
            searchParams.IsBatchAudit = true;
            var list = await GetMyUncompletedPageList(searchParams);
            return list;
        }

        /// <summary>
        /// 获取流程列表(流程监控)对应process/monitorpage接口
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="input">查询参数</param>
        /// <returns></returns>
        //[HttpGet("workflow/process/monitorpage")]
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Monitor)]
        public async Task<PagedResultDto<WorkFlowProcessMonitorDto>> GetMonitorPage(GetMonitorPageInput input)
        {
            //1.获取权限范围内的可监控流程
            var query = _processRepository.GetAll()
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), t => t.Title.Contains(input.Keyword) || t.SchemeName.Contains(input.Keyword))
                .WhereIf(input.CreateUserId.HasValue && input.CreateUserId != Guid.Empty, t => t.CreatorUserId.Equals(input.CreateUserId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.SchemeCode), x => x.SchemeCode.Contains(input.SchemeCode))//根据权限获取相应的数据
                .WhereIf(input.StartDate != null && input.EndDate != null, t => t.CreationTime >= input.StartDate && t.CreationTime <= input.EndDate)//根据创建日期查询
                ;
            //流程状态  作废流程
            if (input.Type == 3)
                query = query.Where(t => t.IsActive == 3);//作废流程
            else if (input.Type == 5)
                query = query.Where(t => t.IsException == 1);//错误流程
            else
            {
                query = query.Where(t => t.IsActive == 1);//错误流程
                if (input.Type == 4)
                    query = query.Where(t => t.IsFinished == 1);
                else if (input.Type == 1)
                    query = query.Where(t => t.IsFinished == 0);
                //查所有流程
                //else//默认查询未结束流程
                //    query = query.Where(t => t.IsFinished == 0);
            }
            //审批人
            if (input.AuditUserId.HasValue && input.AuditUserId != Guid.Empty)
            {
                query = query.Where(t => _taskRepository.GetAll().Where(s => s.UserId == input.AuditUserId && s.State == 1 && s.Type != 2 && t.Id == s.ProcessId).Any());
            }
            //正在运行的流程并且是否离职
            if (input.Type == 1 && input.IsLeave == 1)
            {
                // 获取审批人账号禁用流程
                //先查询已离职，并且用户不在黑名单里面，账号不等于启用的用户数据
                var inactiveUserIds = _userRepository.GetAll()
                                  .Where(u => u.IsActive != true)
                                  .Select(u => u.Id)
                                  .ToList();
                query = query.Where(t =>
                    _taskRepository.GetAll()
                                   .Any(s =>
                                       inactiveUserIds.Contains(s.UserId.Value) &&
                                       s.State == 1 &&
                                       s.Type != 2 &&
                                       t.Id == s.ProcessId)
                );
            }

            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();
            var processIds = list.Select(d => d.Id).ToList();
            if (processIds.Count > 0)
            {
                var taskUnFinishList = await _workFlowTaskSerivce.GetUnFinishTaskListByProIds(processIds);

                var result = list.Select(entity =>
                {
                    var item = ObjectMapper.Map<WorkFlowProcessMonitorDto>(entity);

                    var myTaskUnFinishList = taskUnFinishList.Where(t => t.ProcessId == item.Id);
                    var dic = new Dictionary<Guid, bool>();
                    item.Step = 100;
                    foreach (var unFinishItem in myTaskUnFinishList)
                    {
                        if (!dic.ContainsKey(unFinishItem.UnitId))
                        {
                            dic.Add(unFinishItem.UnitId, true);
                            if (!string.IsNullOrEmpty(item.UnitNames))
                            {
                                item.UnitNames += ",";
                            }
                            item.UnitNames += unFinishItem.UnitName;
                            if (item.Step > unFinishItem.Step)
                            {
                                item.Step = unFinishItem.Step;
                            }
                        }

                        var auditUserIds = item.AuditUserIds;
                        if (string.IsNullOrEmpty(auditUserIds) || auditUserIds.IndexOf(unFinishItem.UserId.ToString()) == -1)
                        {
                            if (!string.IsNullOrEmpty(auditUserIds))
                            {
                                auditUserIds += $",{unFinishItem.UserId}";
                            }
                            else
                            {
                                auditUserIds = unFinishItem.UserId.ToString();
                            }
                            item.AuditUserIds = auditUserIds;
                        }
                    }
                    return item;
                }).ToList();
                return new PagedResultDto<WorkFlowProcessMonitorDto>(totalCount, result);
            }
            return new PagedResultDto<WorkFlowProcessMonitorDto>(0, new List<WorkFlowProcessMonitorDto>());
        }

        /// <summary>
        /// 获取我的流程-workflow/process/mypage
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="input">查询参数</param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowProcessDto>> GetMyWorkFlowPageList(GetMyWorkFlowPageListInput input)
        {
            var list = await this.GetMyPageList(input, 1);
            return list;
        }

        /// <summary>
        /// 获取我的流程
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="input">查询参数</param>
        /// <param name="type"> 1正常2草稿3作废</param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<PagedResultDto<WorkFlowProcessDto>> GetMyPageList(GetMyWorkFlowPageListInput input, int type)
        {
            var query = _processRepository.GetAll();
            if (input.UserId.HasValue && input.UserId != Guid.Empty)
            {
                query = query.Where(t => t.CreatorUserId.Value.Equals(input.UserId));
            }
            else if (type != 3)
            {
                query = query.Where(t => t.CreatorUserId == ADTOSharpSession.GetUserId() || t.UserId == ADTOSharpSession.GetUserId());
            }
            query = query.Where(t => t.IsChild != 1 || t.IsChild == null);
            if (type == 2)
            {
                query = query.Where(t => t.IsActive == 2);
            }
            else
            {
                if (input.Type == 3)
                {
                    query = query.Where(t => t.IsActive == 3);
                }
                else if (input.Type == 1 || input.Type == 4)
                {
                    query = query.Where(t => t.IsActive == 1);

                    if (input.Type == 4)
                    {
                        query = query.Where(t => t.IsActive == 1);
                    }
                    else
                    {
                        query = query.Where(t => t.IsActive == 0);
                    }
                }
                else
                {
                    query = query.Where(t => t.IsActive != 2);
                }
            }
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                query = query.Where(t => t.Title.Contains(input.Keyword) || t.SchemeName.Contains(input.Keyword));
            }

            if (!string.IsNullOrEmpty(input.FormKeyword))
            {
                query = query.Where(t => t.Keyword.Contains(input.FormKeyword));
            }

            if (input.ProcessId.HasValue && input.ProcessId != Guid.Empty)
            {
                query = query.Where(t => t.Id == input.ProcessId);
            }

            if (!string.IsNullOrEmpty(input.Code))
            {
                query = query.Where(t => t.SchemeCode == input.Code);
            }
            if (input.StartDate != null && input.EndDate != null)
            {
                query = query.Where(t => t.CreationTime >= input.StartDate && t.CreationTime <= input.EndDate);
            }
            if (!string.IsNullOrEmpty(input.Where))
            {
                string where = $"ProcessId in ({AESHelper.AesDecrypt(input.Where, "ADTODCloud")})";

            }

            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);

            var totalCount = query.Count();
            var processList = await query.PageBy(input).ToListAsync();
            var processIds = processList.Select(d => d.Id).ToList();
            if (processIds.Count > 0)
            {
                var taskUnFinishList = await _workFlowTaskSerivce.GetUnFinishTaskListByProIds(processIds);
                var result = processList.Select(item =>
                {
                    var processDto = ObjectMapper.Map<WorkFlowProcessDto>(item);
                    var myTaskUnFinishList = taskUnFinishList.Where(t => t.ProcessId == item.Id);
                    var dic = new Dictionary<Guid, bool>();
                    processDto.Step = 100;
                    foreach (var unFinishItem in myTaskUnFinishList)
                    {
                        if (!dic.ContainsKey(unFinishItem.UnitId) && unFinishItem.Type != 2)
                        {
                            dic.Add(unFinishItem.UnitId, true);
                            if (!string.IsNullOrEmpty(processDto.UnitNames))
                            {
                                processDto.UnitNames += ",";
                            }
                            processDto.UnitNames += unFinishItem.UnitName;
                            if (processDto.Step > unFinishItem.Step)
                            {
                                processDto.Step = unFinishItem.Step;
                            }
                        }
                    }
                    return processDto;
                }).ToList();
                return new PagedResultDto<WorkFlowProcessDto>(totalCount, result);
            }
            return null;
        }


        /// <summary>
        /// 获取我的草稿-mydraftpage
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="input">查询参数</param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowProcessDto>> GetMyDraftPage(GetMyWorkFlowPageListInput input)
        {
            var list = await this.GetMyPageList(input, 2);
            return list;
        }
        /// <summary>
        /// 获取我的传阅任务-read/mypage
        /// </summary>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowTaskDto>> GetMyReadPageList(GetMyReadPageListInput input)
        {
            var query = from task in _taskRepository.GetAll()
                        join t3 in _processRepository.GetAll() on task.ProcessId equals t3.Id into t3
                        from process in t3.DefaultIfEmpty()
                        select new { CreationTime = task.CreationTime, task, process, ProcessId = task.ProcessId, IsUrge = task.IsUrge, IsUp = task.IsUp, IsUp2 = task.IsUp2, state = task.State };

            query = query.Where(t => t.task.UserId == ADTOSharpSession.GetUserId())
                .Where(t => t.task.Type == 2)
                .Where(t => t.task.State == 1 || t.task.State == 3)
                .WhereIf(!string.IsNullOrEmpty(input.Keyword), t => t.task.ProcessTitle.Contains(input.Keyword) || t.task.UnitName.Contains(input.Keyword))
                .WhereIf(!string.IsNullOrEmpty(input.Code), t => t.task.ProcessCode == input.Code)
                .WhereIf(input.StartDate != null && input.EndDate != null, t => t.task.CreationTime >= input.StartDate && t.task.CreationTime <= input.EndDate)
                .WhereIf(!string.IsNullOrEmpty(input.FormKeyword), t => t.process.Keyword.Contains(input.FormKeyword))
                .WhereIf(!string.IsNullOrEmpty(input.SchemaName), t => t.process.SchemeName == input.SchemaName)
                .WhereIf(input.UserId.HasValue && input.UserId != Guid.Empty, t => t.process.CreatorUserId == input.UserId);
            if (!string.IsNullOrEmpty(input.Where))
            {
                string where = $"ProcessId in ({AESHelper.AesDecrypt(input.Where, "DCloudADTO")})";
                query = query.Where(where);
            }

            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);
            var totalCount = query.Count();
            var list = await query.PageBy(input).ToListAsync();

            var result = list.Select(item =>
            {
                var dto = ObjectMapper.Map<WorkFlowTaskDto>(item.task);

                dto.PorcessUserId = item.process.CreatorUserId;
                dto.IsFinished = item.process.IsFinished;
                dto.IsDelete = item.process.IsActive;
                dto.Level = item.process.Level;
                dto.SchemeName = item.process.SchemeName;
                return dto;
            }).ToList();
            return new PagedResultDto<WorkFlowTaskDto>(totalCount, result);

        }


        /// <summary>
        /// 获取我的委托任务-process/delegate/mypage
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="input">查询参数</param>
        /// <returns></returns>
        public async Task<PagedResultDto<WorkFlowTaskDto>> GetMyDelegatePageList(GetMyDelegatePageListInput input)
        {
            var expression = PredicateBuilder.New<WorkFlowTask>(p => true);  // 初始化默认条件
            //获取当前用户的委托信息
            var delegateList = await _delegateService.GetByToUserIdList();
            if (delegateList.Count() == 0)
                return new PagedResultDto<WorkFlowTaskDto>();
            int num = 0;
            foreach (var item in delegateList)
            {
                var relationList = await _delegateService.GetRelationList(item.Id);
                string relation = "";
                Guid? userId = item.CreatorUserId;
                foreach (var relationItem in relationList)
                {
                    relation += $"'{relationItem.SchemeInfoCode}'";
                }
                if (num == 0)
                {
                    expression = expression.And(t => t.UserId == userId && relation.Contains("'" + t.ProcessCode + "'"));
                }
                else
                {
                    expression = expression.Or(t => t.UserId == userId && relation.Contains("'" + t.ProcessCode + "'"));
                }
                num++;
            }
            if (num == 0)
                return new PagedResultDto<WorkFlowTaskDto>();

            expression = expression.And(t => t.UserId != ADTOSharpSession.GetUserId());
            expression = expression.And(t => t.State == 1);
            expression = expression.And(t => t.Type != 2);

            if (!string.IsNullOrEmpty(input.Keyword))
            {
                expression = expression.And(t => t.ProcessTitle.Contains(input.Keyword) || t.UnitName.Contains(input.Keyword));
            }
            if (!string.IsNullOrEmpty(input.Code))
            {
                expression = expression.And(t => t.ProcessCode == input.Code);
            }
            if (input.StartDate != null && input.EndDate != null)
            {
                expression = expression.And(t => t.CreationTime >= input.StartDate && t.CreationTime <= input.EndDate);
            }

            string where = string.Empty;

            if (!string.IsNullOrEmpty(input.FormKeyword))
            {
                //where = $"ProcessId in (select Id From WorkFlowProcesses where Keyword Like '%{input.FormKeyword}%'  )";
                expression = expression.And(t => _processRepository.GetAll().Where(q => q.Keyword.Contains(input.FormKeyword)).Select(d => d.Id).Contains(t.ProcessId));
            }

            if (!string.IsNullOrEmpty(input.Where))
            {
                if (!string.IsNullOrEmpty(where))
                {
                    where += " AND ";
                }
                where = $"ProcessId in ({AESHelper.AesDecrypt(input.Where, "ADTODCloud")})";
            }
            var query = _taskRepository.GetAll().AsQueryable().Where(expression);
            if (input.Sorting != null)
                query = query.OrderBy(input.Sorting);

            var totalCount = query.Count();
            var taskList = await query.PageBy(input).ToListAsync();

            List<Guid> processIds = taskList.Select(d => d.ProcessId).ToList();

            if (processIds.Count > 0)
            {
                var processList = await _processRepository.GetAll().Where(t => processIds.Contains(t.Id)).ToListAsync();
                var result = taskList.Select(item =>
                {
                    var task = ObjectMapper.Map<WorkFlowTaskDto>(item);
                    var processEntity = processList.Find(t => t.Id == task.ProcessId);
                    if (processEntity != null)
                    {
                        task.PorcessUserId = processEntity.CreatorUserId;
                        task.IsFinished = processEntity.IsFinished;
                        task.IsDelete = processEntity.IsActive;
                        task.SchemeName = processEntity.SchemeName;
                        task.Level = processEntity.Level;
                    }
                    return task;
                }).ToList();
                return new PagedResultDto<WorkFlowTaskDto>(totalCount, result);
            }
            return new PagedResultDto<WorkFlowTaskDto>(totalCount, ObjectMapper.Map<List<WorkFlowTaskDto>>(taskList));
        }

        /// <summary>
        /// 获取下一节点审批人-process/nextusers
        /// </summary>
        /// <param name="code">流程模板编码</param>
        /// <param name="processId">流程进程主键</param>
        /// <param name="schemeId">流程模板主键</param>
        /// <param name="nodeId">流程节点Id</param>
        /// <param name="operationCode">流程操作代码</param>
        /// <param name="userId">创建人</param>
        /// <param name="nextNodeId">下一审批节点</param>
        /// <returns></returns>
        public async Task<Dictionary<Guid, List<WFUserInfo>>> GetNextAuditors(GetNextAuditorsInput input)
        {
            Dictionary<Guid, List<WFUserInfo>> res = new Dictionary<Guid, List<WFUserInfo>>();
            var userInfo = await UserManager.GetUserByIdAsync(input.UserId);
            var iWFEngine = await Bootstraper(input.Code, input.ProcessId, input.SchemeId, null, null, null, userInfo);
            var taskEntity = await _workFlowTaskSerivce.GetLastTask(input.ProcessId, input.NodeId);
            Guid rejectNode = Guid.Empty;
            if (taskEntity != null && taskEntity.IsReject == 1)
            {
                rejectNode = taskEntity.PrevUnitId;
            }
            if (!string.IsNullOrEmpty(input.NextNodeId))
            {
                var nextNodeIds = input.NextNodeId.Split(',');
                foreach (var nextNodeIdItem in nextNodeIds)
                {
                    //下一部需要执行的任务
                    var taskList = await iWFEngine.GetTask(input.NodeId, 0, input.OperationCode, Guid.Parse(nextNodeIdItem), rejectNode, null, true);
                    foreach (var item in taskList)
                    {
                        if (item.Type == 1 || item.Type == 3 || item.Type == 4 || item.Type == 5)
                        {
                            if (!res.ContainsKey(item.UnitId))
                            {
                                res.Add(item.UnitId, new List<WFUserInfo>());
                            }

                            if (item.User != null)
                            {
                                res[item.UnitId].Add(item.User);
                            }
                        }
                    }
                }
            }
            else
            {
                if (input.Code != "disagree" && taskEntity?.Type == 6)// 1.不是驳回 2.加签审批; (加签处理)
                {
                    var signTaskList = await _workFlowTaskSerivce.GetSignTaskList(taskEntity.FirstId);
                    var otherSignTaskList = signTaskList.Where(t => t.Id != taskEntity.Id);
                    if (otherSignTaskList.Count() > 0)
                    {
                        if (otherSignTaskList.Any(t => t.State == 2))
                        {
                            // 串行
                            var newTask = otherSignTaskList.FirstOrDefault(t => t.Sort == taskEntity.Sort + 1);
                            if (!res.ContainsKey(newTask.UnitId))
                            {
                                res.Add(newTask.UnitId, new List<WFUserInfo>());
                            }
                            res[newTask.UnitId].Add(new WFUserInfo()
                            {
                                Id = newTask.UserId.Value,
                                Name = newTask.UserName
                            });
                            return res;
                        }
                        if (otherSignTaskList.Any(t => t.State == 1))
                        {
                            return res;
                        }
                    }
                }
                // 下一部需要执行的任务
                var taskList = await iWFEngine.GetTask(input.NodeId, taskEntity?.IsRejectBackOld, input.OperationCode, null, rejectNode, null, true);
                foreach (var item in taskList)
                {
                    if (item.Type == 1 || item.Type == 3 || item.Type == 4 || item.Type == 5 || item.Type == 6)
                    {
                        if (!res.ContainsKey(item.UnitId))
                        {
                            res.Add(item.UnitId, new List<WFUserInfo>());
                        }
                        if (item.User != null)
                        {
                            res[item.UnitId].Add(item.User);
                        }
                    }
                }
            }
            return res;
        }


        /// <summary>
        /// 流程模板初始化
        /// </summary>
        /// <param name="code">流程模板</param>
        /// <param name="processId">流程实例</param>
        /// <param name="schemeId">流程模板ID</param>
        /// <param name="parentProcessId">父级流程id</param>
        /// <param name="parentTaskId">父级流程任务ID</param>
        /// <param name="title">流程标题</param>
        /// <param name="userInfo">提交人用户信息</param>
        /// <param name="nextUsers">下一个节点审批人</param>
        /// <returns></returns>
        private async Task<IWorkFlowEngineAppService> Bootstraper(string code, Guid processId, Guid? schemeId, Guid? parentProcessId, Guid? parentTaskId, string title, User userInfo,
            Dictionary<Guid, string> nextUsers = null)
        {
            WFEngineConfig wfEngineConfig = new WFEngineConfig();
            WFParams wfParams = new WFParams();
            wfEngineConfig.Params = wfParams;
            var currenUser = await this.GetCurrentUserAsync();
            wfParams.CurrentUser = new WFUserInfo()
            {
                Id = currenUser.Id,
                Account = currenUser.UserName,
                Name = currenUser.Name,
                CompanyId = currenUser.CompanyId ?? Guid.Empty,
                DepartmentId = currenUser.DepartmentId ?? Guid.Empty
            };

            //根据流程编码查询流程模板信息
            if (!string.IsNullOrEmpty(code))
            {
                var schemeInfo = await _workFlowSchemeAppService.GetSchemeInfoEntityByCode(code);
                if (schemeInfo != null)
                {
                    var scheme = await _workFlowSchemeAppService.GetSchemeEntity(schemeInfo.SchemeId);
                    if (scheme != null)
                    {
                        wfParams.Scheme = scheme.Content;
                        wfParams.SchemeCode = code;
                        wfParams.SchemeId = schemeInfo.SchemeId;
                        wfParams.SchemeName = schemeInfo.Name;
                        wfParams.ProcessId = processId;
                        wfParams.HasInstance = false;
                        wfParams.Title = title;

                        wfParams.CreateUser = new WFUserInfo()
                        {
                            Id = userInfo.Id,
                            Account = userInfo.UserName,
                            Name = userInfo.Name,
                            CompanyId = userInfo.CompanyId ?? Guid.Empty,
                            DepartmentId = userInfo.DepartmentId ?? Guid.Empty,
                            //PostId = userInfo.Post ?? Guid.Empty//没用查询用户岗位
                        };
                    }
                    else
                    {
                        throw new UserFriendlyException(string.Format("#NotLog#无法获取对应的流程模板【{0}】", code));
                    }
                }
                else
                {
                    throw new UserFriendlyException(string.Format("#NotLog#无法获取对应的流程模板【{0}】", code));
                }
            }
            //根据流程ID获取流程信息
            else if (!processId.IsEmpty())
            {
                var processEntity = await _processRepository.GetAsync(processId);
                if (processEntity != null)
                {
                    var data = await _workFlowSchemeAppService.GetSchemeEntity(processEntity.SchemeId ?? Guid.Empty);
                    if (data != null)
                    {
                        wfParams.Scheme = data.Content;
                        wfParams.SchemeCode = processEntity.SchemeCode;
                        wfParams.SchemeId = processEntity.SchemeId ?? Guid.Empty;
                        wfParams.SchemeName = processEntity.SchemeName;
                        wfParams.IsChild = processEntity.IsChild == 1;
                        wfParams.ParentProcessId = processEntity.ParentProcessId ?? Guid.Empty;
                        wfParams.ParentTaskId = parentTaskId != null && parentTaskId != Guid.Empty ? processEntity.ParentTaskId.Value : parentTaskId;
                        wfParams.ProcessId = processId;
                        wfParams.HasInstance = true;
                        wfParams.IsStart = processEntity.IsStart == 1 ? 1 : 0;
                        wfParams.Title = processEntity.Title;
                        wfParams.IsFinished = processEntity.IsFinished == 1 ? 1 : 0;
                        if (processEntity.IsAgain == 1)
                        {
                            wfParams.State = 1;
                        }
                        else if (processEntity.IsFinished == 1)
                        {
                            wfParams.State = 2;
                        }
                        else if (processEntity.IsActive == 2)
                        {
                            wfParams.State = 3;
                        }
                        else if (processEntity.IsActive == 3)
                        {
                            wfParams.State = 4;
                        }
                        var user = await UserManager.GetUserByIdAsync(processEntity.UserId);
                        wfParams.CreateUser = new WFUserInfo()
                        {
                            Id = processEntity.CreatorUserId.Value,
                            Account = user.UserName,
                            Name = user.Name,
                            CompanyId = user.CompanyId ?? Guid.Empty,
                            DepartmentId = user.DepartmentId ?? Guid.Empty,
                            //PostId = processEntity.F_Keyword2
                        };
                    }
                    else
                    {
                        throw new UserFriendlyException(string.Format("#NotLog#无法获取流程模板【{0}】", processEntity.SchemeId));
                    }
                }
                //根据流程模板Id查询流程模板信息
                else if (schemeId.HasValue && schemeId != Guid.Empty)
                {
                    var data = await _workFlowSchemeAppService.GetSchemeEntity(schemeId.Value);
                    if (data != null)
                    {
                        var schemeInfo = await _workFlowSchemeAppService.GetSchemeInfoEntity(data.SchemeInfoId);
                        if (schemeInfo != null)
                        {
                            wfParams.Scheme = data.Content;
                            wfParams.SchemeCode = schemeInfo.Code;
                            wfParams.SchemeId = schemeId.Value;
                            wfParams.SchemeName = schemeInfo.Name;
                            wfParams.IsChild = !parentProcessId.IsEmpty();
                            wfParams.ParentProcessId = parentProcessId;
                            wfParams.ParentTaskId = parentTaskId;
                            wfParams.ProcessId = processId;
                            wfParams.HasInstance = false;
                            wfParams.Title = title;
                            wfParams.CreateUser = new WFUserInfo()
                            {
                                Id = userInfo.Id,
                                Account = userInfo.UserName,
                                Name = userInfo.Name,
                                CompanyId = userInfo.CompanyId.Value,
                                DepartmentId = userInfo.DepartmentId.Value,
                                //PostId = userInfo.F_Post
                            };
                        }
                        else
                        {
                            throw new UserFriendlyException(string.Format("#NotLog#无法获取流程模板信息【{0}】", schemeId));
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException(string.Format("#NotLog#无法获取流程模板信息【{0}】", schemeId));
                    }
                }
                else if (string.IsNullOrEmpty(wfParams.Scheme))
                {
                    throw new UserFriendlyException(string.Format("#NotLog#无法获取实例数据【{0}】", processId));
                }
            }
            wfParams.NextUsers = nextUsers;

            // 注册委托方法
            wfEngineConfig.GetAwaitTaskList = GetAwaitTaskList;
            wfEngineConfig.GetUserList = GetUserList;
            wfEngineConfig.GetSystemUserList = GetSystemUserList;
            wfEngineConfig.GetPrevTaskUserList = GetPrevTaskUserList;
            wfEngineConfig.GetPrevTaskUser = GetPrevTaskUser;
            wfEngineConfig.IsCountersignAgree = IsCountersignAgree;
            wfEngineConfig.GetPrevUnitId = GetPrevUnitId;

            // 如果需要初始化配置
            _flowEngineAppService.Initialize(wfEngineConfig);
            return _flowEngineAppService;
        }

        /// <summary>
        /// 获取等待任务
        /// </summary>
        /// <param name="processId">流程进程ID</param>
        /// <param name="unitId">流程单元ID</param>
        /// <returns></returns>
        private async Task<List<WFTask>> GetAwaitTaskList(Guid processId, Guid unitId)
        {
            List<WFTask> res = new List<WFTask>();
            var list = await _workFlowTaskSerivce.GetAwaitTaskList(new GetAwaitTaskListInput() { ProcessId = processId, UnitId = unitId });
            foreach (var item in list)
            {
                res.Add(GetWFTask(item));
            }
            return res;
        }
        /// <summary>
        /// 获取设置人员
        /// </summary>
        /// <param name="auditorList">人员配置信息</param>
        /// <param name="createUser">流程发起人</param>
        /// <param name="processId">流程进程实例ID</param>
        /// <param name="myNode">当前节点</param>
        /// <param name="preNode">上一个节点</param>
        /// <param name="startNode">开始节点</param>
        /// <returns></returns>
        [UnitOfWork(isTransactional: false, scope: TransactionScopeOption.RequiresNew)]
        private async Task<List<WFUserInfo>> GetUserList(List<WorkFlowAuditor> auditorList, WFUserInfo createUser, Guid processId, WorkFlowUnit myNode, WorkFlowUnit preNode, WorkFlowUnit startNode)
        {
            var res = new List<WFUserInfo>();
            //获取当前用户
            var currentUser = await this.GetCurrentUserAsync();
            var postId = await UserManager.GetCurrentPostId();
            List<Guid> userIds = new List<Guid>();
            foreach (var item in auditorList)
            {
                switch (item.Type)
                {
                    case "1"://岗位
                        var rlist = await _userPostRepository.GetAll().Where(q => q.PostId.Equals(item.Id)).ToListAsync();
                        foreach (var ritem in rlist)
                        {
                            userIds.Add(ritem.UserId);
                        }
                        break;
                    case "2"://角色
                        var rlist2 = await _userRoleRepository.GetAll().Where(q => q.RoleId.Equals(item.Id)).ToListAsync();
                        List<Guid> userIds2 = new List<Guid>();
                        foreach (var ritem in rlist2)
                        {
                            userIds2.Add(ritem.UserId);
                        }
                        var userList2 = await _userAppService.GetListByKeyValues(userIds2);
                        foreach (var user2 in userList2)
                        {
                            switch (item.Condition)
                            {
                                case "1"://同一个部门
                                    if (user2.DepartmentId == currentUser.DepartmentId)
                                    {
                                        res.Add(GetWFUserInfo(user2));
                                    }
                                    break;
                                case "2"://同一个公司
                                    if (user2.DepartmentId == currentUser.CompanyId)
                                    {
                                        res.Add(GetWFUserInfo(user2));
                                    }
                                    break;
                                case "3":
                                    //发起人上级
                                    // 获取当前用户的岗位
                                    // 节点审批人岗位
                                    var postList2 = await _userPostRepository.GetAll().Where(q => q.UserId.Equals(user2.Id)).Select(d => d.Id).ToListAsync();
                                    if (await _postAppService.IsUp(postId, postList2))
                                    {
                                        res.Add(GetWFUserInfo(user2));
                                    }
                                    break;
                                case "4":
                                    //发起人下级
                                    // 获取当前用户的岗位
                                    // 节点审批人岗位
                                    var postList4 = await _userPostRepository.GetAll().Where(q => q.UserId.Equals(user2.Id)).Select(d => d.Id).ToListAsync();
                                    if (await _postAppService.IsDown(postId, postList4))
                                    {
                                        res.Add(GetWFUserInfo(user2));
                                    }
                                    break;
                                default:
                                    res.Add(GetWFUserInfo(user2));
                                    break;
                            }
                        }
                        break;
                    case "3"://用户
                        userIds.Add(item.Id);
                        break;
                    case "4"://上下级
                        int level = Convert.ToInt32(item.Level);
                        IEnumerable<Guid> postList;
                        if (level < 6)
                        {
                            postList = await _postAppService.GetUpIdList(postId, level);
                        }
                        else
                        {
                            level = level - 5;
                            postList = await _postAppService.GetDownIdList(postId, level);
                        }
                        var userRelationList4 = await _postAppService.GetUserByPostId(new Authorization.Posts.Dto.GetByUserIdListByUserIdsInput() { ObjectIdList = postList.ToList() });
                        foreach (var userRelationItem in userRelationList4)
                        {
                            userIds.Add(userRelationItem.UserId);
                        }
                        break;
                    case "5"://节点执行人
                        if (item.Enforcer == startNode.Id)
                        {
                            res.Add(createUser);
                        }
                        else if (item.Enforcer == preNode.Id && !ADTOSharpSession.GetUserId().IsEmpty())
                        {
                            userIds.Add(ADTOSharpSession.GetUserId());
                        }
                        else
                        {
                            var taskList = await _workFlowTaskSerivce.GetLastFinishTaskList(processId, item.Id);
                            foreach (var task in taskList)
                            {
                                userIds.Add(task.UserId ?? Guid.Empty);
                            }
                        }
                        break;
                    case "6"://数据库表字段
                             //审批人员类型 1.岗位 2.角色 3.用户 4.上下级 5.节点执行人 6.数据库表字段 7.发起人部门 8.上一审批人部门 9.特定部门 11 上一审批人公司 12 特定公司 13 sql语句
                        string userSql = string.Format("select [{0}] from {1} where [{2}] = @processId ", item.AuditorField.Replace(",", "],["), item.Table, item.Rfield);

                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("processId", processId);
                        DataTable dt = await _dataBaseService.FindTable(userSql, parameters);
                        foreach (DataRow row in dt.Rows)
                        {
                            var auditorFields = item.AuditorField.Split(",");
                            foreach (var name in auditorFields)
                            {
                                var userIdString = row[name.ToLower()].ToString();
                                if (!string.IsNullOrEmpty(userIdString))
                                {
                                    var userIdList = userIdString.Split(",");
                                    foreach (var userIdItem in userIdList)
                                    {
                                        userIds.Add(Guid.Parse(userIdItem));
                                    }
                                }
                            }
                        }
                        break;
                    case "7"://发起人部门-获取当前部门负责人-此处数据获取还需更改，具体参照原项目
                        var departmentlist = await _userRelationAppService.GetUserIdList(createUser.DepartmentId.ToString());
                        if (departmentlist.Count() > 0)
                        {
                            foreach (var ritem in departmentlist)
                            {
                                if (ritem.AttrCode == item.Condition)
                                {
                                    userIds.Add(Guid.Parse(ritem.UserId));
                                }
                            }
                        }
                        else
                        {
                            var department1 = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = createUser.DepartmentId });
                            if (department1 != null && department1.ManagerUserId.HasValue)
                            {
                                userIds.Add(department1.ManagerUserId.Value);
                            }
                        }

                        break;
                    case "8"://上一审批人部门
                        var departmentId = await UserManager.GetCurrentDepartmentId();
                        var departmentlist2 = await _userRelationAppService.GetUserIdList(departmentId.ToString());
                        if (departmentlist2.Count() > 0)
                        {
                            foreach (var ritem in departmentlist2)
                            {
                                if (ritem.AttrCode == item.Condition)
                                {
                                    userIds.Add(Guid.Parse(ritem.UserId));
                                }
                            }
                        }
                        else
                        {
                            var department1 = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = createUser.DepartmentId });
                            if (department1 != null && department1.ManagerUserId.HasValue)
                            {
                                var parentDepartment = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = department1.ParentId ?? Guid.Empty });
                                if (parentDepartment != null && parentDepartment.ManagerUserId.HasValue)
                                    userIds.Add(parentDepartment.ManagerUserId.Value);
                            }
                        }

                        break;
                    case "9"://特定部门
                        var departmentlist3 = await _userRelationAppService.GetUserIdList(item.Department.ToString());
                        if (departmentlist3.Count() > 0)
                        {
                            foreach (var ritem in departmentlist3)
                            {
                                if (ritem.AttrCode == item.Condition)
                                {
                                    userIds.Add(Guid.Parse(ritem.UserId));
                                }
                            }
                        }
                        else
                        {
                            var department2 = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = item.Department });
                            if (department2 != null && department2.ManagerUserId.HasValue)
                            {
                                userIds.Add(department2.ManagerUserId.Value);
                            }
                        }

                        break;

                    case "10"://发起人公司
                        var companylist = await _userRelationAppService.GetUserIdList(createUser.CompanyId.ToString());
                        if (companylist.Count() > 0)
                        {
                            foreach (var ritem in companylist)
                            {
                                if (ritem.AttrCode == item.Condition)
                                {
                                    userIds.Add(Guid.Parse(ritem.UserId));
                                }
                            }
                        }
                        else
                        {
                            var company = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = createUser.CompanyId });
                            if (company != null && company.ManagerUserId.HasValue)
                            {
                                userIds.Add(company.ManagerUserId.Value);
                            }
                        }
                        break;
                    case "11": // 上一审批人公司
                        var companyId = await UserManager.GetCurrentCompanyId();
                        var companylist2 = await _userRelationAppService.GetUserIdList(companyId.ToString());
                        foreach (var ritem in companylist2)
                        {
                            if (ritem.AttrCode == item.Condition)
                            {
                                userIds.Add(Guid.Parse(ritem.UserId));
                            }
                        }

                        break;
                    case "12"://特定公司
                        //var company2 = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = item.Company });
                        //if (company2 != null && company2.ManagerUserId.HasValue)
                        //{
                        //    userIds.Add(company2.ManagerUserId.Value);
                        //}
                        var companylist3 = await _userRelationAppService.GetUserIdList(item.Company.ToString());
                        foreach (var ritem in companylist3)
                        {
                            if (ritem.AttrCode == item.Condition)
                            {
                                userIds.Add(Guid.Parse(ritem.UserId));
                            }
                        }
                        break;
                    case "13"://sql语句
                        if (!string.IsNullOrEmpty(item.StrSql))
                        {
                            var currentUser3 = await this.GetCurrentUserAsync();
                            var paramData = new
                            {
                                processId,
                                userId = createUser.Id,
                                userAccount = createUser.Account,
                                companyId = createUser.CompanyId,
                                departmentId = createUser.DepartmentId,
                                userId2 = currentUser3.Id,
                                userAccount2 = currentUser3.UserName,
                                companyId2 = currentUser3.CompanyId,
                                departmentId2 = currentUser3.DepartmentId,
                                nodeId = myNode.Id,
                                preNodeId = preNode.Id
                            };

                            DataTable dt2 = await _dataBaseService.FindTable(item.StrSql, paramData);
                            foreach (DataRow row in dt2.Rows)
                            {
                                var userIdString = row[0].ToString();
                                if (!string.IsNullOrEmpty(userIdString))
                                {
                                    var userIdList = userIdString.Split(",");
                                    foreach (var userIdItem in userIdList)
                                    {
                                        userIds.Add(Guid.Parse(userIdItem));
                                    }
                                }
                            }
                        }
                        break;
                    case "14":
                        //新增-直属上级---因为目前用户里面没有直属上级这一栏，所以代码暂时空置
                        var userInfo = await _userRepository.GetAsync(createUser.Id);
                        if (userInfo != null && userInfo.ManagerId.HasValue)
                        {
                            userIds.Add(userInfo.ManagerId.Value);
                        }
                        else
                        {
                            //如果用户里面没有直属上级则用员工里面的直属上级
                            var employeeInfo = await _employeeRepository.FirstOrDefaultAsync(q => q.UserId.Equals(createUser.Id));
                            if (employeeInfo != null && employeeInfo.ManagerId != Guid.Empty)
                                userIds.Add(employeeInfo.ManagerId.Value);
                        }
                        break;
                    case "15"://新增-部门负责人
                        var department = await _organizationUnitAppService.GetOrganizationUnitsByIdAsync(new EntityDto<Guid>() { Id = createUser.DepartmentId });
                        if (department != null && department.ManagerUserId.HasValue)
                        {
                            userIds.Add(department.ManagerUserId.Value);
                        }
                        break;
                }
            }
            var userList = await _userAppService.GetListByKeyValues(userIds);
            foreach (var user in userList)
            {
                //获取用户代理人
                var users = await _userDelegationAppService.GetWorkFlowDelegatedUsers(user.Id, DateTime.Now);
                foreach (var deleteUser in users)
                {
                    res.Add(new WFUserInfo()
                    {
                        Id = deleteUser.UserId,
                        Name = deleteUser.Name,
                        Account = deleteUser.Username,
                        DepartmentId = deleteUser.DepartmentId,
                        CompanyId = deleteUser.CompanyId,
                        IsAdmin = false
                    });
                }
                res.Add(GetWFUserInfo(user));
            }
            return res.GroupBy(t => t.Id).Select(t => t.First()).ToList();
        }

        /// <summary>
        /// 获取系统管理员
        /// </summary>
        /// <returns></returns>
        private async Task<List<WFUserInfo>> GetSystemUserList()
        {
            var res = new List<WFUserInfo>();
            //获取当前管理员下面的所有用户
            //var userList = await UserManager.GetUsersInAdminRoleAsync();
            var user = await UserManager.GetAdminAsync();
            //res.Add(GetWFUserInfo(user));
            res.Add(new WFUserInfo()
            {
                Id = user.Id,
                Name = user.Name,
                Account = user.UserName,
                DepartmentId = user.DepartmentId ?? Guid.Empty,
                CompanyId = user.CompanyId ?? Guid.Empty,
                IsAdmin = true
            });

            return res;
        }
        /// <summary>
        /// 获取单元上一次的审批人
        /// </summary>
        /// <param name="processId">流程进程ID</param>
        /// <param name="unitId">流程单元ID</param>
        /// <returns></returns>
        private async Task<List<WFUserInfo>> GetPrevTaskUserList(Guid processId, Guid unitId)
        {
            var res = new List<WFUserInfo>();
            var list = await _workFlowTaskSerivce.GetLastTaskList(processId, unitId);
            foreach (var item in list)
            {
                if (!item.UserId.IsEmpty())
                {
                    res.Add(new WFUserInfo()
                    {
                        Id = item.UserId ?? Guid.Empty,
                        Name = item.UserName,
                        CompanyId = item.UserCompanyId ?? Guid.Empty,
                        DepartmentId = item.UserDepartmentId ?? Guid.Empty,
                        IsAgree = item.IsAgree == 1,
                        Sort = item.Sort,
                        State = (int)item.State,
                        IsSign = item.Type == 6,
                        ChildProcessId = item.ChildProcessId ?? Guid.Empty
                    });
                }
            }
            return res;
        }

        /// <summary>
        /// 获取单元上一次的审批人
        /// </summary>
        /// <param name="processId">流程进程ID</param>
        /// <param name="unitId">流程单元ID</param>
        /// <returns></returns>
        private async Task<WFUserInfo> GetPrevTaskUser(Guid processId, Guid unitId)
        {
            var task = await _workFlowTaskSerivce.GetLastTask(processId, unitId);
            if (task != null)
            {
                return new WFUserInfo()
                {
                    Id = task.UserId ?? Guid.Empty,
                    Name = task.UserName,
                    CompanyId = task.UserCompanyId ?? Guid.Empty,
                    DepartmentId = task.UserDepartmentId ?? Guid.Empty,
                    IsAgree = task.IsAgree == 1,
                    Sort = task.Sort,
                    State = (int)task.State,
                    ChildProcessId = task.ChildProcessId ?? Guid.Empty
                };
            }
            return null;
        }
        /// <summary>
        /// 判断会签是否通过
        /// </summary>
        /// <param name="processId">流程进程ID</param>
        /// <param name="unitId">流程单元ID</param>
        /// <returns></returns>
        private async Task<bool> IsCountersignAgree(Guid processId, Guid unitId)
        {
            var list = await _workFlowTaskLogAppService.GetLogList1(processId, unitId);
            if (list.Count() == 0)
            {
                return false;
            }
            return list.FirstOrDefault().IsAgree == 1;
        }
        /// <summary>
        /// 获取上一个流入节点（不是驳回流入的）
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="unitId"></param>
        /// <param name="fromId"></param>
        /// <returns></returns>
        private async Task<Guid> GetPrevUnitId(Guid processId, Guid unitId, Guid fromId)
        {
            var data = await _workFlowTaskSerivce.GetLastNotRejectTask(processId, unitId);
            if (data != null)
            {
                return data.PrevUnitId;
            }
            data = await _workFlowTaskSerivce.GetLastRejectTask(processId, unitId);
            if (data != null)
            {
                return data.PrevUnitId;
            }
            // 用于自动跳过的节点
            var data2 = await _workFlowTaskLogAppService.GetLogList1(processId, unitId);
            data2 = data2.Where(t => t.PrevUnitId != fromId);
            if (data2.Count() > 0)
            {
                return data2.FirstOrDefault().PrevUnitId ?? Guid.Empty;
            }
            return Guid.Empty;
        }
        /// <summary>
        /// 获取流程任务数据
        /// </summary>
        /// <param name="wfTaskEntity"></param>
        /// <returns></returns>
        private WFTask GetWFTask(WorkFlowTaskDto wfTaskEntity)
        {
            var res = new WFTask()
            {
                UnitId = wfTaskEntity.UnitId,
                Name = wfTaskEntity.UnitName,
                PrevUnitId = wfTaskEntity.PrevUnitId,
                PrevUnitName = wfTaskEntity.PrevUnitName,
                Token = wfTaskEntity.Token,
                Type = (int)wfTaskEntity.Type
            };
            return res;
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        private WFUserInfo GetWFUserInfo(UserDto userEntity)
        {
            var res = new WFUserInfo()
            {
                Id = userEntity.Id,
                Name = userEntity.Name,
                Account = userEntity.UserName,
                DepartmentId = userEntity.DepartmentId ?? Guid.Empty,
                CompanyId = userEntity.CompanyId ?? Guid.Empty,
                IsAdmin = false// userEntity.SecurityLevel == 1
            };
            return res;
        }
        /// <summary>
        /// 获取子流程
        /// </summary>
        /// <param name="parentProcessId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkFlowProcessDto>> GetChildList(Guid parentProcessId)
        {
            var list = await _processRepository.GetAll().Where(t => t.ParentProcessId == parentProcessId).ToListAsync();
            return ObjectMapper.Map<IEnumerable<WorkFlowProcessDto>>(list);
        }
        /// <summary>
        /// 获取表单信息
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        //public async Task<string> GetFormTableInfo(Guid processId)
        //{

        //}
        #endregion


        #region 获取任务
        /// <summary>
        /// 获取任务-/process/task/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<WFTaskDto> GetTask(EntityDto<Guid> input)
        {
            var res = new WFTaskDto();
            var task = await _taskRepository.GetAsync(input.Id);
            res.Task = ObjectMapper.Map<WorkFlowTaskDto>(task);
            if (res.Task == null)
            {
                throw new UserFriendlyException("找不到当前任务");
            }
            // 设置为已读状态
            if (res.Task.ReadStatus != 1)
            {
                if (res.Task.Type == 2)
                {
                    await ReadWorkFlow(new EntityDto<Guid>() { Id = res.Task.Id });
                    res.Task.State = 3;
                }
                else
                {
                    await _workFlowTaskSerivce.UpdateTaskReadStatus(res.Task.Id);
                }
                res.Task.ReadStatus = 1;
            }

            if (res.Task.Type == 3)
            {
                var process = await _processRepository.GetAsync(res.Task.ChildProcessId.Value);
                res.Process = ObjectMapper.Map<WorkFlowProcessDto>(process);
                var scheme = await _schemeRepository.GetAsync(res.Task.ChildSchemeId.Value);
                res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(scheme);
                if (res.Process != null)
                {
                    res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(res.Task.ChildProcessId.Value);
                }
            }
            else
            {
                var process = await _processRepository.GetAsync(res.Task.ProcessId);
                res.Process = ObjectMapper.Map<WorkFlowProcessDto>(process);
                var scheme = await _schemeRepository.GetAsync(res.Process.SchemeId.Value);
                res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(scheme);
                res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(res.Task.ProcessId);
            }
            // 更新同一节点的任务
            var taskList = (List<WorkFlowTaskDto>)res.Tasks;
            var myTask = taskList.Find(t => t.Id != res.Task.Id && t.ReadStatus != 1 && t.UnitId == res.Task.UnitId && res.Task.UserId == t.UserId);
            if (myTask != null && myTask.ReadStatus != 1)
            {
                if (myTask.Type == 2)
                {
                    await ReadWorkFlow(new EntityDto<Guid>() { Id = myTask.Id });
                    res.Tasks = taskList.FindAll(t => t.Id != myTask.Id);
                }
                else
                {
                    await _workFlowTaskSerivce.UpdateTaskReadStatus(myTask.Id);
                }

                myTask.ReadStatus = 1;
            }


            if (res.Task.Type == 3)
            {
                if (res.Process != null)
                {
                    res.Logs = await _workFlowTaskLogAppService.GetLogListByProcessId(res.Task.ChildProcessId.Value);
                }
            }
            else
            {
                res.Logs = await _workFlowTaskLogAppService.GetLogListByProcessId(res.Task.ProcessId);
            }
            return res;
        }

        /// <summary>
        /// 获取任务-根据流程Id
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<WFTaskDto> GetTaskByProcessId(EntityDto<Guid> input)
        {
            var res = new WFTaskDto();
            var process = await _processRepository.GetAsync(input.Id);
            if (process == null || process.Id == Guid.Empty)
            {
                throw new UserFriendlyException("找不到当前流程信息");
            }
            var task = await _taskRepository.GetAll().Where(q => q.ProcessId.Equals(process.Id)).OrderByDescending(o => o.CreationTime).FirstOrDefaultAsync();
            res.Task = ObjectMapper.Map<WorkFlowTaskDto>(task);
            if (res.Task == null)
            {
                throw new UserFriendlyException("找不到当前任务");
            }
            // 设置为已读状态
            if (res.Task.ReadStatus != 1)
            {
                if (res.Task.Type == 2)
                {
                    await ReadWorkFlow(new EntityDto<Guid>() { Id = res.Task.Id });
                    res.Task.State = 3;
                }
                else
                {
                    await _workFlowTaskSerivce.UpdateTaskReadStatus(res.Task.Id);
                }
                res.Task.ReadStatus = 1;
            }
            //子流程
            if (res.Task.Type == 3)
            {
                var childprocess = await _processRepository.GetAsync(res.Task.ChildProcessId.Value);
                res.Process = ObjectMapper.Map<WorkFlowProcessDto>(childprocess);
                var scheme = await _schemeRepository.GetAsync(res.Task.ChildSchemeId.Value);
                res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(scheme);
                if (res.Process != null)
                {
                    res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(res.Task.ChildProcessId.Value);
                }
            }
            else
            {
                //var process = await _processRepository.GetAsync(res.Task.ProcessId);
                res.Process = ObjectMapper.Map<WorkFlowProcessDto>(process);
                var scheme = await _schemeRepository.GetAsync(res.Process.SchemeId.Value);
                res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(scheme);
                res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(res.Task.ProcessId);
            }
            // 更新同一节点的任务
            var taskList = (List<WorkFlowTaskDto>)res.Tasks;
            var myTask = taskList.Find(t => t.Id != res.Task.Id && t.ReadStatus != 1 && t.UnitId == res.Task.UnitId && res.Task.UserId == t.UserId);
            if (myTask != null && myTask.ReadStatus != 1)
            {
                if (myTask.Type == 2)
                {
                    await ReadWorkFlow(new EntityDto<Guid>() { Id = myTask.Id });
                    res.Tasks = taskList.FindAll(t => t.Id != myTask.Id);
                }
                else
                {
                    await _workFlowTaskSerivce.UpdateTaskReadStatus(myTask.Id);
                }

                myTask.ReadStatus = 1;
            }


            if (res.Task.Type == 3)
            {
                if (res.Process != null)
                {
                    res.Logs = await _workFlowTaskLogAppService.GetLogListByProcessId(res.Task.ChildProcessId.Value);
                }
            }
            else
            {
                res.Logs = await _workFlowTaskLogAppService.GetLogListByProcessId(res.Task.ProcessId);
            }
            return res;
        }
        /// <summary>
        /// 获取任务根据token获取流程任务--workflow/process/task/token/{token}
        /// </summary>
        /// <param name="token">任务token</param>
        /// <returns></returns>
        public async Task<WFTaskDto> GetTaskByToken(string token)
        {
            var res = new WFTaskDto();
            res.Task = await _workFlowTaskSerivce.GetEntityByToken(token, ADTOSharpSession.GetUserId());
            if (res.Task == null)
            {
                throw (new Exception("#NotLog#找不到当前任务！"));
            }

            // 设置为已读状态
            if (res.Task.ReadStatus != 1)
            {
                if (res.Task.Type == 2)
                {
                    await this.ReadWorkFlow(new EntityDto<Guid>() { Id = res.Task.Id });
                    res.Task.State = 3;
                }
                else
                {
                    await _workFlowTaskSerivce.UpdateTaskReadStatus(res.Task.Id);
                }
                res.Task.ReadStatus = 1;
            }

            if (res.Task.Type == 3)
            {
                res.Process = ObjectMapper.Map<WorkFlowProcessDto>(await _processRepository.FirstOrDefaultAsync(res.Task.ChildProcessId.Value));
                res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(await _formSchemeAppService.GetSchemeEntity(res.Task.ChildSchemeId.Value.ToString()));
                if (res.Process != null)
                {
                    res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(res.Task.ChildProcessId.Value);
                }
            }
            else
            {
                res.Process = ObjectMapper.Map<WorkFlowProcessDto>(await _processRepository.FirstOrDefaultAsync(res.Task.ProcessId));

                res.Scheme = ObjectMapper.Map<WorkFlowSchemeDto>(await _formSchemeAppService.GetSchemeEntity(res.Task.ChildSchemeId.Value.ToString()));

                res.Tasks = await _workFlowTaskSerivce.GetUnFinishTaskList(res.Task.ProcessId);
            }

            // 更新同一节点的任务
            var taskList = (List<WorkFlowTaskDto>)res.Tasks;
            var myTask = taskList.Find(t => t.Id != res.Task.Id && t.ReadStatus != 1 && t.UnitId == res.Task.UnitId && res.Task.UserId == t.UserId);
            if (myTask != null && myTask.ReadStatus != 1)
            {
                if (myTask.Type == 2)
                {
                    await this.ReadWorkFlow(new EntityDto<Guid>() { Id = myTask.Id });
                    res.Tasks = taskList.FindAll(t => t.Id != myTask.Id);
                }
                else
                {
                    await _workFlowTaskSerivce.UpdateTaskReadStatus(myTask.Id);
                }

                myTask.ReadStatus = 1;
            }


            if (res.Task.Type == 3)
            {
                if (res.Process != null)
                {
                    res.Logs = await _workFlowTaskLogAppService.GetLogList(res.Task.ChildProcessId.Value);
                }
            }
            else
            {
                res.Logs = await _workFlowTaskLogAppService.GetLogList(res.Task.ProcessId);
            }
            return res;
        }
        #endregion

        #region 获取流程流水号
        /// <summary>
        /// 获取流程流水号
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public async Task<int> GetIndexByCode(string code)
        {
            var list = await _processRepository.GetAll().Where(q => q.SchemeCode.Equals(code)).ToListAsync();
            if (list.Count > 0 && list[0].Index != null)
            {
                return (int)list[0].Index;
            }
            return 0;
        }
        #endregion

        #region 暂存

        /// <summary>
        /// 保存草稿-对应process/draft接口
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        public async Task<IActionResult> SaveDraft(WorkFlowDraftInput dto)
        {
            await this.SaveDraft(dto.ProcessId, dto.SchemeCode, dto.UserId, dto.RProcessId, dto.SecretLevel, dto.Title);
            return new JsonResult(new
            {
                Message = L("保存草稿成功"),
                Success = true
            });
        }
        /// <summary>
        /// 保存草稿
        /// </summary>
        /// <param name="processId">流程进程主键</param>
        /// <param name="schemeCode">流程模板编码</param>
        /// <param name="userId">创建人</param>
        /// <param name="rProcessId">关联流程ID</param>
        /// <param name="secretLevel">密级</param>
        /// <param name="title">流程标题</param>
        /// <returns></returns>
        private async Task<IActionResult> SaveDraft(Guid processId, string schemeCode, Guid userId, Guid? rProcessId, int? secretLevel, string title = "")
        {
            // 判断当前流程进程是否有保存过
            var processEntity = await _processRepository.GetAll().Where(t => t.Id.Equals(processId)).FirstOrDefaultAsync();
            if (processEntity == null || processEntity.Id == Guid.Empty)
            {
                // 创建草稿，已经存在不做处理
                var schemeInfo = await _workFlowSchemeAppService.GetSchemeInfoEntityByCode(schemeCode);
                WorkFlowProcess wfProcessEntity = new WorkFlowProcess()
                {
                    Id = processId,
                    SchemeCode = schemeCode,
                    SchemeId = schemeInfo.SchemeId,
                    SchemeName = schemeInfo.Name,
                    Title = title,
                    IsActive = 2,
                    IsAgain = 0,
                    IsFinished = 0,
                    IsChild = 0,
                    IsStart = 0,
                    SecretLevel = secretLevel,
                    UserId = ADTOSharpSession.GetUserId(),
                    RprocessId = rProcessId
                };
                if (string.IsNullOrEmpty(title))
                {
                    //await _iCache.Lock("wfCreate" + schemeInfo.Code, async _ =>
                    //{
                    var userInfo = await this.GetCurrentUserAsync();
                    string custmerTitle = await GetCustmerTitle(processId, schemeInfo, userInfo);
                    if (!string.IsNullOrEmpty(custmerTitle))
                    {
                        wfProcessEntity.Title = custmerTitle;
                    }
                    else
                    {
                        wfProcessEntity.Title = $"{userInfo.Name}-{wfProcessEntity.SchemeName}";
                    }
                    await _processRepository.InsertAsync(wfProcessEntity);
                    //});
                }
                else
                {
                    await _processRepository.InsertAsync(wfProcessEntity);
                }
            }
            else if (!string.IsNullOrEmpty(title) && processEntity.Title != title)
            {
                processEntity.Title = title;
                await _processRepository.UpdateAsync(processEntity);
            }
            return new JsonResult(new
            {
                Message = L("保存草稿成功"),
                Success = true
            });
        }

        /// <summary>
        /// 删除草稿-workflow/process/draft/{id}
        /// </summary>
        /// <param name="id">流程id</param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteDraft(EntityDto<Guid> input)
        {
            var iWFEngine = await Bootstraper(string.Empty, input.Id, null, null, null, string.Empty, null);
            await ExecuteScript(input.Id, "deleteDraft", "", iWFEngine.CreateUser,
                iWFEngine.WFScheme.DeleteDraftType,
                iWFEngine.WFScheme.DeleteDraftDbCode,
                iWFEngine.WFScheme.DeleteDraftDbSQL,
                iWFEngine.WFScheme.DeleteDraftUrl,
                iWFEngine.WFScheme.DeleteDraftIOCName);
            await _processRepository.DeleteAsync(input.Id);

            return new JsonResult(new
            {
                Message = L("删除草稿成功"),
                Success = true
            });
        }


        #endregion

        #region 手动创建流程-项目里面静态表单调用
        /// <summary>
        /// 手动创建流程-表示项目里面手动调用
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateWorkFlowAsync(CreateWorkFlowInput input)
        {
            try
            {
                //先暂存流程
                await this.SaveDraft(input.ProcessId, input.SchemeCode, input.UserId, input.RProcessId, input.SecretLevel, input.Title);
                return await this.CreateAsync(new WorkFlowCreateInput() { UserId = input.UserId, ProcessId = input.ProcessId, Title = input.Title, Level = input.Level, SecretLevel = input.SecretLevel, RProcessId = input.RProcessId ?? Guid.Empty });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region 创建流程
        /// <summary>
        /// 创建流程-process/create
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        public async Task<IActionResult> CreateAsync(WorkFlowCreateInput dto)
        {

            // 设置创建人信息
            var userInfo = await UserManager.GetUserByIdAsync(dto.UserId);
            //获取用户岗位
            var userPost = await UserManager.GetCurrentPostId(dto.UserId);
            //如果UserId等于当前用户Id
            if (dto.UserId == ADTOSharpSession.GetUserId())
            {
                var currentUser = await this.GetCurrentUserAsync();
                userPost = await UserManager.GetCurrentPostId();
                userInfo.CompanyId = currentUser.CompanyId;
                userInfo.DepartmentId = currentUser.DepartmentId;
            }
            var iWFEngine = await Bootstraper(dto.SchemeCode, dto.ProcessId, null, null, null, dto.Title, userInfo, dto.NextUsers);
            if (iWFEngine.Config.Params.State != 3)
            {
                return new JsonResult(new
                {
                    Message = L("TheCurrentProcessHasBeenSubmitted"),
                    Success = false
                });
            }
            await CreateFlow(iWFEngine, dto.RProcessId, dto.Des, dto.FileId, dto.Level, dto.SecretLevel);

            return new JsonResult(new
            {
                Message = L("TheProcessCreationWasSuccessful"),
                Success = true
            });
        }
        /// <summary>
        /// 创建流程
        /// </summary>
        /// <param name="iWFEngine">流程标题</param>
        /// <param name="rProcessId">关联流程实例</param>
        /// <param name="des">流程提交说明</param>
        /// <param name="fileId">流程提交附件</param>
        /// <param name="level">流程等级</param>
        /// <param name="secretLevel">密级</param>
        /// <param name="prevToken">父级流程任务</param>
        /// <param name="taskId">父级流程任务id</param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task CreateFlow(IWorkFlowEngineAppService iWFEngine, Guid? rProcessId, string des, string fileId, int? level, int? secretLevel, string prevToken = "create", string taskId = "create")
        {
            // 下一部需要执行的任务
            var taskList = await iWFEngine.GetTask(iWFEngine.StartNode.Id, 0);
            var processEntity = await GetWFProcessEntity(iWFEngine.Config.Params);
            processEntity.SecretLevel = secretLevel;
            processEntity.RprocessId = rProcessId;
            processEntity.Level = level;
            processEntity.AllAuditTime = iWFEngine.StartNode.AllAuditTime;
            // 获取表单数据
            processEntity.Keyword = await _formSchemeAppService.GetFormKeyword(iWFEngine.Config.Params.ProcessId, iWFEngine.WFScheme);
            List<WorkFlowTaskDto> myTaskList;
            try
            {
                processEntity.CreationTime = DateTime.Now;
                if (taskList.FindIndex(t => t.Type == 100) != -1)
                {
                    processEntity.IsFinished = 1;
                    processEntity.FinishTime = DateTime.Now;
                    processEntity.ProcessMinutes = 0;
                }
                if (iWFEngine.Config.Params.HasInstance)
                {
                    await _processRepository.UpdateAsync(processEntity);
                }
                else
                {
                    await _processRepository.InsertAsync(processEntity);
                }
                myTaskList = await ExecuteWFTaskEntity(taskList, iWFEngine, iWFEngine.Config.Params.ProcessId, prevToken, 0, taskId);
                foreach (var item in myTaskList)
                {
                    item.ProcessTitle = processEntity.Title;
                    await _workFlowTaskSerivce.Add(item);
                }
                WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
                {
                    OperationCode = "create",
                    OperationName = "提交",
                    TaskType = 0,
                    IsLast = 1,
                    Remark = des,
                    FileId = fileId,
                    ProcessId = iWFEngine.Config.Params.ProcessId,
                    UnitId = iWFEngine.StartNode.Id,
                    UnitName = string.IsNullOrEmpty(iWFEngine.StartNode.Name) ? "开始节点" : iWFEngine.StartNode.Name
                };
                await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);

                //执行节点回调函数
                await ExecuteInvokeMethod(iWFEngine.StartNode, iWFEngine, processEntity.Id, des, "Create");
                // 脚本执行
                var scriptTaskList = myTaskList.FindAll(t => t.Type == 10);
                foreach (var item in scriptTaskList)
                {
                    await ExecuteScript(item, "create", "", iWFEngine.Config.Params.ProcessId, null, iWFEngine.CreateUser, iWFEngine, null);
                }

                // 子流程创建
                var autoChildList = myTaskList.FindAll(t => t.Type == 3 && t.IsAuto == 1);
                foreach (var childItem in autoChildList)
                {
                    // 自动创建子流程或者再发起子流程
                    var node = iWFEngine.GetNode(childItem.UnitId);
                    var userInfo = await UserManager.GetUserByIdAsync(childItem.UserId.Value);
                    var childIWFEngine = await Bootstraper(string.Empty, childItem.ChildProcessId ?? Guid.Empty, node.WfVersionId, childItem.ProcessId, childItem.Id, "", userInfo, null);

                    await CreateFlow(childIWFEngine, Guid.Empty, "", "", level, secretLevel);// 子流程没关联流程递归处理

                    // 更新任务状态
                    childItem.IsAgree = 1;
                    childItem.State = 8;
                    await _workFlowTaskSerivce.UpdateTask(childItem);
                    await CreateTaskLog(childItem, "createsub", "子流程提交", "", "", null, "");
                }
                // 如果流程结束且是子流程需要跳转到父级流程
                if (iWFEngine.Config.Params.IsChild)
                {
                    if (processEntity.IsFinished == 1)
                    {
                        await FinishToParent(iWFEngine.Config.Params.ParentTaskId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="myParams">流程运行参数</param>
        /// <returns></returns>
        private async Task<WorkFlowProcess> GetWFProcessEntity(WFParams myParams)
        {
            var wfProcessEntity = await _processRepository.GetAsync(myParams.ProcessId) ?? new WorkFlowProcess();
            wfProcessEntity.SchemeCode = myParams.SchemeCode;
            wfProcessEntity.SchemeId = myParams.SchemeId;
            wfProcessEntity.SchemeName = myParams.SchemeName;
            wfProcessEntity.Title = myParams.Title;
            wfProcessEntity.IsActive = 1;
            wfProcessEntity.IsAgain = 0;
            wfProcessEntity.IsFinished = 0;
            wfProcessEntity.IsChild = myParams.IsChild ? 1 : 0;
            wfProcessEntity.ParentProcessId = myParams.ParentProcessId;
            wfProcessEntity.ParentTaskId = myParams.ParentTaskId;
            wfProcessEntity.IsStart = myParams.IsStart;
            wfProcessEntity.IsCancel = 1;
            wfProcessEntity.CreatorUserId = myParams.CreateUser.Id;
            wfProcessEntity.CreateUserName = myParams.CreateUser.Name;
            wfProcessEntity.CreateUserAccount = myParams.CreateUser.Account;
            wfProcessEntity.CreateUserCompanyId = myParams.CreateUser.CompanyId;
            wfProcessEntity.CreateUserDepartmentId = myParams.CreateUser.DepartmentId;
            wfProcessEntity.Keyword2 = myParams.CreateUser.PostId + "";
            wfProcessEntity.UserId = ADTOSharpSession.GetUserId();
            if (string.IsNullOrEmpty(wfProcessEntity.Title))
            {
                wfProcessEntity.Title = $"{myParams.CreateUser.Name}-{myParams.SchemeName}";
            }
            return wfProcessEntity;
        }

        /// <summary>
        /// 处理任务
        /// </summary>
        /// <param name="list">任务列表</param>
        /// <param name="iWFEngine"></param>
        /// <param name="processId">流程进程实例</param>
        /// <param name="prevToken">任务Token</param>
        /// <param name="IsRejectBackOld">是否按原路驳回</param>
        /// <param name="taskId">任务id</param>
        /// <returns></returns>
        private async Task<List<WorkFlowTaskDto>> ExecuteWFTaskEntity(List<WFTask> list, IWorkFlowEngineAppService iWFEngine, Guid processId, string prevToken, int? IsRejectBackOld, string taskId = "")
        {
            var res = new List<WorkFlowTaskDto>();

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.MessageType))
                {
                    //发送消息
                    var taskType = "";
                    switch (item.Type)
                    {
                        case 1:
                        case 5:
                            taskType = "审批";
                            break;
                        case 2:
                            taskType = "查阅";
                            break;
                        case 3:
                            if (!item.IsAuto)
                            {
                                taskType = "子流程提交";
                            }
                            break;
                        case 4:
                            taskType = "重新提交";
                            break;
                        case 101:
                            taskType = "审批结束";
                            item.Token = processId.ToString();
                            break;
                    }
                    if (!string.IsNullOrEmpty(taskType))
                    {
                        var msg = $"【{taskType}】{iWFEngine.Config.Params.Title},{item.Name}";
                        var userList = new List<Guid>();
                        userList.Add(item.User.Id);
                        await _messageAppService.SendMsg(new Massages.Dto.SendMassageDto()
                        {
                            Code = "IMWF",
                            UserIdList = userList,
                            Content = msg,
                            MessageType = item.MessageType,
                            ContentId = item.Token
                        });
                    }
                }
                switch (item.Type)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        var userTask = new WorkFlowTaskDto
                        {
                            Type = item.Type,
                            ProcessId = processId,
                            Token = item.Token,
                            UnitId = item.UnitId,
                            NodeCode = item.NodeCode,
                            UnitName = item.Name,
                            PrevUnitId = item.PrevUnitId,
                            PrevUnitName = item.PrevUnitName,
                            PrevToken = prevToken,
                            PrevTaskId = taskId,
                            UserId = item.User.Id,
                            UserName = item.User.Name,
                            UserDepartmentId = item.User.DepartmentId,
                            UserCompanyId = item.User.CompanyId,
                            State = item.User.IsAwait ? 2 : 1,
                            Sort = item.User.Sort,
                            IsReject = item.IsReject ? 1 : 0,
                            Step = item.Step,
                            AuditTime = item.AuditTime,
                            IsUp = 0,
                            IsUp2 = 0,
                            TimeoutInterval = item.OvertimeMessageInterval,
                            TimeoutStrategy = item.OvertimeMessageType,
                            IsBatchAudit = item.IsBatchAudit ? 1 : 0,
                            ProcessUserId = iWFEngine.CreateUser.Id,
                            ProcessUserName = iWFEngine.CreateUser.Name,
                            ProcessCode = iWFEngine.Config.Params.SchemeCode,
                            ProcessTitle = iWFEngine.Config.Params.Title,
                            ChildProcessId = item.ChildProcessId,
                            ChildSchemeId = item.WfVersionId,
                            IsAuto = item.IsAuto ? 1 : 0,
                            IsRejectBackOld = IsRejectBackOld

                        };
                        if (item.IsOvertimeMessage && item.OvertimeMessageStart > 0)
                        {
                            userTask.TimeoutNotice = DateTime.Now.AddHours(item.OvertimeMessageStart);
                        }

                        if (item.IsOvertimeGo && item.OvertimeGo > 0)
                        {
                            userTask.TimeoutAction = DateTime.Now.AddHours(item.OvertimeGo);
                        }

                        if (item.Type == 3)
                        {
                            userTask.ChildProcessId = item.ChildProcessId;
                            // 子流程 执行脚本
                            res.Add(new WorkFlowTaskDto
                            {
                                Type = 10,
                                ProcessId = processId,
                                Token = item.Token,
                                UnitId = item.UnitId,
                                NodeCode = item.NodeCode,
                                UnitName = item.Name,
                                PrevUnitId = item.PrevUnitId,
                                PrevUnitName = item.PrevUnitName,
                                PrevTaskId = taskId,
                                PrevToken = prevToken,
                                ChildProcessId = item.ChildProcessId
                            });
                        }
                        if (item.Type == 6)
                        {
                            userTask.FirstId = Guid.Parse(item.Token);// 并不是实际的，虚拟一个，方便后续处理
                        }
                        res.Add(userTask);
                        break;
                    case 10:
                        // 执行脚本
                        res.Add(new WorkFlowTaskDto
                        {
                            Type = item.Type,
                            ProcessId = processId,
                            Token = item.Token,
                            UnitId = item.UnitId,
                            NodeCode = item.NodeCode,
                            UnitName = item.Name,
                            PrevUnitId = item.PrevUnitId,
                            PrevUnitName = item.PrevUnitName,
                            PrevTaskId = taskId,
                            PrevToken = prevToken
                        });
                        break;
                    case 21:
                        res.Add(new WorkFlowTaskDto
                        {
                            Type = item.Type,
                            ProcessId = processId,
                            Token = item.Token,
                            UnitId = item.UnitId,
                            NodeCode = item.NodeCode,
                            UnitName = item.Name,
                            PrevUnitId = item.PrevUnitId,
                            PrevUnitName = item.PrevUnitName,
                            PrevTaskId = taskId,
                            PrevToken = prevToken,
                            State = 1,
                        });
                        break;
                    case 22:
                        await _workFlowTaskSerivce.CloseTask(processId, item.UnitId, 21, Guid.Parse(taskId));
                        break;
                    case 23:
                    case 24:
                    case 27:
                        WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
                        {
                            Remark = "系统任务",
                            TaskType = item.Type,
                            Token = item.Token,
                            ProcessId = processId,
                            UnitId = item.UnitId,
                            UnitName = item.Name,
                            PrevUnitId = item.PrevUnitId,
                            PrevUnitName = item.PrevUnitName,

                        };
                        await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);
                        break;
                    case 26:
                        // 更新任务状态
                        await _workFlowTaskSerivce.UpdateLast(processId, item.UnitId);
                        break;
                }
            }
            return res;
        }
        #endregion

        #region 再次创建流程
        /// <summary>
        /// 重新创建流程
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        //[HttpPost("workflow/process/createAgain")]
        public async Task<string> CreateAgain(WFProcessDto dto)
        {
            var iWFEngine = await Bootstraper(string.Empty, dto.ProcessId, null, null, null, null, null, dto.NextUsers);
            var unList = await _workFlowTaskSerivce.GetUnFinishTaskList(dto.ProcessId);
            var taskEntity = unList.FirstOrDefault(t => t.State == 1 && t.UnitId == iWFEngine.StartNode.Id);
            if (taskEntity == null)
                throw new UserFriendlyException(L("UnableToFindTheResubmissionProcessTask"));
            // 获取表单数据
            string keyword = await _formSchemeAppService.GetFormKeyword(dto.ParentTaskId, iWFEngine.WFScheme);
            // 下一部需要执行的任务
            var taskList = await iWFEngine.GetTask(iWFEngine.StartNode.Id, taskEntity.IsRejectBackOld, "", null, taskEntity.PrevUnitId);
            List<WorkFlowTaskDto> myTaskList;
            try
            {
                var process = await _processRepository.GetAsync(dto.ProcessId);
                process.Keyword = keyword;
                process.IsCancel = 1;
                process.IsAgain = 0;
                await _processRepository.UpdateAsync(process);
                //await _processRepository.UpdateAsync(new WorkFlowProcess { Id = dto.ProcessId, Keyword = keyword, IsCancel = 1, IsAgain = 0 });
                await _workFlowTaskSerivce.UpdateAsync(new WorkFlowTaskDto { Id = taskEntity.Id, State = 3 });
                // 更新上一个流转过来的任务，提示他无法被撤销
                await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntity.ProcessId, taskEntity.PrevTaskId);
                myTaskList = await ExecuteWFTaskEntity(taskList, iWFEngine, dto.ProcessId, taskEntity.Token, 0, taskEntity.Id.ToString());
                foreach (var item in myTaskList)
                {
                    await _workFlowTaskSerivce.Add(item);
                }
                await CreateTaskLog(taskEntity, "", "重新提交", dto.Des, "", dto.FileId, "");
                // 脚本执行
                var scriptTaskList = myTaskList.Where(t => t.Type == 10);
                foreach (var item in scriptTaskList)
                {
                    await ExecuteScript(item, "createAgain", dto.Des, dto.ProcessId, null, iWFEngine.CreateUser, iWFEngine, null);
                }
                // 子流程创建
                var autoChildList = myTaskList.FindAll(t => t.Type == 3 && t.IsAuto == 1);
                foreach (var childItem in autoChildList)
                {
                    // 自动创建子流程或者再发起子流程
                    var node = iWFEngine.GetNode(childItem.UnitId);
                    var userInfo = await UserManager.GetUserByIdAsync(childItem.UserId.Value);
                    var childIWFEngine = await Bootstraper(string.Empty, childItem.ChildProcessId.Value, node.WfVersionId, childItem.ProcessId, childItem.Id, "", userInfo, null);
                    WorkFlowProcess parentProcessEntity = null;

                    if (parentProcessEntity == null)
                    {
                        parentProcessEntity = await _processRepository.FirstOrDefaultAsync(dto.ProcessId);
                    }
                    await CreateFlow(childIWFEngine, null, "", "", parentProcessEntity.Level, parentProcessEntity.SecretLevel, childItem.Token, childItem.Id.ToString());
                    // 更新任务状态
                    childItem.IsAgree = 1;
                    childItem.State = 8;
                    await _workFlowTaskSerivce.UpdateTask(childItem);
                    await CreateTaskLog(childItem, "createsub", "子流程提交", "", "", null, "");
                }
                return L("TheProcessCreationWasSuccessful");
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region 流程审核
        /// <summary>
        /// 流程审批-workflow/process/audit/{id}
        /// </summary>
        public async System.Threading.Tasks.Task AuditFlow(WorkFlowAuditInput input)
        {
            try
            {
                await AuditWorkFLow(input.TaskId, input.Code, input.Name, input.Des, input.NextUsers, input.StampImg, input.StampPassWord, input.NextId, input.FileId, input.Tag, input.IsRejectBackOld);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(L("流程审批失败，请看错误原因：" + ex.Message));
            }

        }

        /// <summary>
        /// 批量审批（只有同意和不同意）-workflow/process/audits/{code}
        /// </summary>
        /// <param name="code">操作码</param>
        /// <param name="taskIdList">任务id串</param>
        public async Task<IActionResult> AuditFlows(WorkFlowAuditsInput input)
        {
            Guid[] taskIdList = input.Ids
           .Split(new[] { '，', ',' }, StringSplitOptions.RemoveEmptyEntries)
           .Select(p => Guid.TryParse(p.Trim(), out Guid guid) ? guid : Guid.Empty)
           .ToArray();

            foreach (var taskId in taskIdList)
            {
                string operationName = input.Code == "agree" ? "同意" : "不同意";
                await AuditWorkFLow(taskId, input.Code, operationName, "批量审批", null, "", "", null, "", "", 0);
            }
            return new JsonResult(new
            {
                Message = L("流程审批成功"),
                Success = true
            });
        }
        /// <summary>
        /// 撤销审批-workflow/process/audit/revoke/{id}
        /// </summary>
        /// <param name="id">流程实例主键</param>
        /// <param name="taskId">流程任务主键</param>
        /// <returns></returns>
        public async Task<IActionResult> RevokeAudit(RevokeAuditInput input)
        {
            try
            {
                await this.RevokeAudit1(input.processId, input.TaskId);
                return new JsonResult(new
                {
                    Message = L("流程审批成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw (new UserFriendlyException($"撤销审批报错：{ex.Message}"));
            }
        }
        /// <summary>
        /// 审批流程
        /// </summary>
        /// <param name="taskId">流程任务主键</param>
        /// <param name="code">流程审批操作码agree 同意 disagree 不同意 lrtimeout 超时</param>
        /// <param name="name">流程审批操名称</param>
        /// <param name="des">审批意见</param>
        /// <param name="nextUsers">下一节点指定审批人</param>
        /// <param name="stampImg">盖章图片</param>
        /// <param name="stampPassWord">盖章图片密码</param>
        /// <param name="nextId">下一个审批节点</param>
        /// <param name="fileId">审批附件</param>
        /// <param name="tag">审批要点</param>
        /// <param name="IsRejectBackOld">驳回是否原路返回</param>
        private async System.Threading.Tasks.Task AuditWorkFLow(Guid taskId, string code, string name, string des,
            Dictionary<Guid, string> nextUsers, string stampImg, string stampPassWord, Guid? nextId, string fileId, string tag, int? IsRejectBackOld)
        {
            try
            {
                var taskEntity = await _taskRepository.GetAsync(taskId);
                if (taskEntity == null)
                {
                    throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                }
                else if (taskEntity.State != 1)
                {
                    throw (new UserFriendlyException("#NotLog#该任务未被激活！"));
                }

                await LockExtensions.Locking(taskEntity.ProcessId, async () =>
                {
                    var task = await _taskRepository.GetAsync(taskId);
                    if (task == null)
                    {
                        throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                    }
                    else if (task.State != 1)
                    {
                        throw (new UserFriendlyException("#NotLog#该任务未被激活！"));
                    }
                    var taskDto = ObjectMapper.Map<WorkFlowTaskDto>(task);
                    var user = await UserManager.GetUserByIdAsync(ADTOSharpSession.GetUserId());
                    if (code != "disagree" && task.Type == 6)// 1.不是驳回 2.加签审批; (加签处理)
                    {
                        var signTaskList = await _workFlowTaskSerivce.GetSignTaskList(task.FirstId);
                        var otherSignTaskList = signTaskList.Where(t => t.Id != taskId).ToList();
                        if (otherSignTaskList.Count() > 0)
                        {
                            if (otherSignTaskList.FindIndex(t => t.State == 2) != -1)
                            {
                                // 串行
                                var newTask = otherSignTaskList.Find(t => t.Sort == task.Sort + 1);
                                try
                                {
                                    // 更新任务状态
                                    task.State = 3;
                                    await _taskRepository.UpdateAsync(task);
                                    newTask.State = 1;
                                    newTask.CreationTime = DateTime.Now;
                                    newTask.CreatorUserId = ADTOSharpSession.UserId;
                                    newTask.CreatorUserName = user.UserName;
                                    await _workFlowTaskSerivce.UpdateAsync(newTask);
                                    // 填写日志
                                    await CreateTaskLog(taskDto, code, name, des, "", fileId, tag);

                                    // 更新上一个流转过来的任务，提示他无法被撤销
                                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, taskDto.PrevTaskId);
                                }
                                catch
                                {
                                    throw;
                                }
                                return;
                            }

                            if (otherSignTaskList.FindIndex(t => t.State == 1) != -1)
                            {
                                try
                                {
                                    // 更新任务状态
                                    task.State = 3;
                                    await _taskRepository.UpdateAsync(task);
                                    // 填写日志
                                    await CreateTaskLog(taskDto, code, name, des, "", fileId, tag);
                                    // 更新上一个流转过来的任务，提示他无法被撤销
                                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, task.PrevTaskId);
                                }
                                catch
                                {
                                    throw;
                                }
                                return;
                            }
                        }
                    }

                    stampImg = await _stampAppService.ToWfImg(stampImg, stampPassWord);
                    var iWFEngine = await Bootstraper("", task.ProcessId, null, null, null, string.Empty, null, nextUsers);

                    // 1.判断任务类型 1 普通任务 ，3子流程 6 加签审批任务
                    if (task.Type == 1 || task.Type == 3 || task.Type == 6)
                    {
                        await AuditNode(iWFEngine, taskDto, code, name, des, stampImg, fileId, tag, IsRejectBackOld, nextId);
                    }
                    else if (task.Type == 5)//会签任务
                    {
                        await AuditNodeByCountersign(iWFEngine, taskDto, code, name, des, stampImg, fileId, tag);
                    }
                    else
                    {
                        throw (new UserFriendlyException($"#NotLog#该任务无法审批,任务类型为【{task.Type}】！"));
                    }
                    //执行节点回调函数
                    var node = iWFEngine.GetNode(task.UnitId);
                    await ExecuteInvokeMethod(node, iWFEngine, task.ProcessId, des, taskDto.IsAgree.ToString());
                    // 更新消息
                    await _messageAppService.VirtualDeleteByContentId(task.Token);
                    // 发送消息
                    var msg = $"【提醒】{iWFEngine.Config.Params.Title},{task.UnitName}已被处理";
                    var userList = new List<Guid>();
                    userList.Add(iWFEngine.CreateUser.Id);
                    var startNode = iWFEngine.StartNode;
                    await _messageAppService.SendMsg(new Massages.Dto.SendMassageDto()
                    {
                        Code = "IMWF",
                        UserIdList = userList,
                        Content = msg,
                        MessageType = startNode.MessageType,
                        ContentId = iWFEngine.Config.Params.ProcessId.ToString()
                    });

                    if (iWFEngine.Config.Params.IsChild)
                    {
                        var processEntity = await _processRepository.GetAsync(iWFEngine.Config.Params.ParentProcessId.Value);
                        processEntity.IsStart = 1;
                        processEntity.IsCancel = 0;
                        await _processRepository.UpdateAsync(processEntity);
                    }
                });

            }
            catch (Exception ex)
            {
                throw (new UserFriendlyException($"#NotLog#该任务无法审批,任务类型为【{ex.Message}】！"));
            }

        }


        /// <summary>
        /// 撤回审批
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="taskId">流程任务主键</param>
        /// <param name="isParent">是否是父级流程</param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task RevokeAudit1(Guid processId, Guid? taskId, bool isParent = false)
        {
            await LockExtensions.Locking(processId.ToString(), async _ =>
            {

                var iWFEngine = await Bootstraper(string.Empty, processId, null, null, null, string.Empty, null);
                if (iWFEngine.Config.Params.IsFinished == 1)
                {
                    throw (new UserFriendlyException("#NotLog#当前流程已结束无法撤销！"));
                }

                WorkFlowTaskDto taskEntity;
                WorkFlowProcess processEntity = await _processRepository.FirstOrDefaultAsync(d => d.Id.Equals(processId));
                if (taskId == null || taskId.IsEmpty())
                { //撤销重新提交任务
                    if (processEntity.IsCancel != 1)
                    {
                        throw (new UserFriendlyException("#NotLog#当前流程无法撤销！"));
                    }
                    var myTaskList = await _workFlowTaskSerivce.GetLastTaskList(processId, iWFEngine.StartNode.Id);
                    taskEntity = myTaskList.FirstOrDefault();
                    processEntity.IsAgain = 1;
                    processEntity.IsCancel = 0;
                }
                else
                {
                    taskEntity = await _workFlowTaskSerivce.GetAsync(new EntityDto<Guid>() { Id = taskId.Value });
                    if (taskEntity.State == 1)
                    {
                        throw (new UserFriendlyException("#NotLog#当前任务无法撤销！"));
                    }
                    // 如果是加签审批或者沟通审批，就需要特殊处理
                    if (taskEntity.Type == 6)
                    {
                        // 加签给其他加签人的情况
                        var toTaskEntiy = await _workFlowTaskSerivce.GetEntityPrevTaskId(taskEntity.Id.ToString());

                        if (toTaskEntiy != null && toTaskEntiy.State != 1)
                        {
                            throw (new UserFriendlyException("#NotLog#再次加签的任务已被处理，无法撤销！"));
                        }
                        // 原始审批任务
                        var myTaskEntiy = await _workFlowTaskSerivce.GetAsync(new EntityDto<Guid>() { Id = taskEntity.FirstId });
                        if (myTaskEntiy == null)
                        {
                            throw (new UserFriendlyException("#NotLog#无法找到加签前的审批任务！"));
                        }
                        if (myTaskEntiy.State != 1)
                        {
                            throw (new UserFriendlyException("#NotLog#加签前的审批任务已被处理无法撤销！"));
                        }
                        // 允许撤销
                        try
                        {
                            // 删除所有生成的任务
                            if (toTaskEntiy != null)
                            {
                                await _taskRepository.DeleteAsync(toTaskEntiy.Id);
                            }
                            myTaskEntiy.State = 5;
                            await _workFlowTaskSerivce.UpdateAsync(myTaskEntiy);
                            // 删除所有生成的日志
                            await _workFlowTaskLogAppService.DeleteLogByProIdAndTskId(processId, taskEntity.Id);
                            await _workFlowTaskLogAppService.DeleteSystemLog(processId, taskEntity.UnitId);
                        }
                        catch (UserFriendlyException)
                        {
                            throw;
                        }

                        return;
                    }

                    // 沟通
                    if (taskEntity.Type == 7)
                    {
                        throw (new UserFriendlyException("#NotLog#沟通无法撤销！"));
                    }

                    var taskLogEntity = await _workFlowTaskLogAppService.GetLogEntity(new EntityDto<Guid>() { Id = taskId.Value });
                    if (taskLogEntity == null || taskLogEntity.IsCancel != 1)
                    {
                        throw (new UserFriendlyException("#NotLog#当前任务无法撤销！"));
                    }
                    processEntity.IsFinished = 0;
                    processEntity.FinishTime = null;
                    processEntity.IsAgain = 0;
                }
                try
                {
                    // 如果生成任务里有子流程任务
                    if (taskEntity.Type != 3)
                    {
                        var childTaskList = await _workFlowTaskSerivce.GetTaskList(processId, 3, taskEntity.Id.ToString());
                        foreach (var item in childTaskList)
                        {
                            if (item.IsAuto == 1)
                            {
                                if (item.State == 1)
                                {
                                    throw new UserFriendlyException("#NotLog#当前任务无法撤销！");
                                }

                                var childEntity = await this.GetChildByTaskId(item.Id);
                                if (childEntity.IsFinished == 1)
                                {
                                    throw new UserFriendlyException("#NotLog#当前任务无法撤销！");
                                }

                                var taskLogEntity = await _workFlowTaskLogAppService.GetLogEntity(new EntityDto<Guid>() { Id = item.Id });
                                if (taskLogEntity == null || taskLogEntity.IsCancel != 1)
                                {
                                    throw (new UserFriendlyException("#NotLog#当前任务无法撤销！"));
                                }
                                if (childEntity.IsStart == 1)
                                {
                                    await RevokeSubAudit(childEntity.Id, item.Id);
                                }
                                else
                                {
                                    var res = await RevokeFlow(new EntityDto<Guid>() { Id = childEntity.Id });
                                    if (!res)
                                    {
                                        throw (new Exception("#NotLog#当前任务无法撤销！"));
                                    }
                                }
                                await _workFlowTaskLogAppService.DeleteLogByProIdAndTskId(processId, item.Id);
                                await _workFlowTaskLogAppService.DeleteSystemLog(processId, item.UnitId);
                            }

                        }
                    }
                    else if (!isParent)
                    {
                        var childEntity = await this.GetChildByTaskId(taskEntity.Id);
                        if (childEntity.IsFinished == 1)
                        {
                            throw new UserFriendlyException("#NotLog#当前任务无法撤销！");
                        }
                        if (childEntity.IsStart == 1)
                        {
                            await RevokeSubAudit(childEntity.Id, taskEntity.Id);
                        }
                        else
                        {
                            var res = await RevokeFlow(new EntityDto<Guid>() { Id = childEntity.Id });
                            if (!res)
                            {
                                return;
                            }
                        }
                    }

                    // 获取脚本任务
                    var scriptList = await _workFlowTaskSerivce.GetTaskList(processId, 10, taskEntity.Id.ToString());
                    foreach (var item in scriptList)
                    {
                        await ExecuteRevokeScript(item.UnitId, item.UnitName, processId, iWFEngine.CreateUser, iWFEngine);
                        // 删除脚本任务执行日志
                        await _workFlowTaskLogAppService.DeleteLogByTaskId(item.Id);
                    }
                    // 删除所有生成的任务
                    await _workFlowTaskSerivce.DeleteTaskByProIdAndTskId(processId, taskEntity.Id.ToString());

                    // 如果是撤销转办任务
                    if (taskEntity.State == 6)
                    {
                        // 删除转办任务
                        await _workFlowTaskSerivce.DeleteByFirstId(taskEntity.Id);
                    }

                    // 删除所有生成的日志
                    await _workFlowTaskLogAppService.DeleteLogByProIdAndTskId(processId, taskEntity.Id);
                    await _workFlowTaskLogAppService.DeleteSystemLog(processId, taskEntity.UnitId);

                    // 更新等待任务状态
                    await _workFlowTaskSerivce.OpenTask(processId, taskEntity.UnitId, 21, taskEntity.Id);

                    if (isParent)
                    {
                        taskEntity.State = 8;
                    }
                    else
                    {
                        taskEntity.State = 1;
                    }

                    await _workFlowTaskSerivce.UpdateAsync(taskEntity);

                    await _processRepository.UpdateAsync(processEntity);

                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            });
        }

        /// <summary>
        /// 审批（暂存）--process/saveaudit/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<IActionResult> SaveAuditFlow(EntityDto<Guid> input)
        {
            try
            {
                var taskEntity = await _taskRepository.GetAsync(input.Id);
                if (taskEntity == null)
                {
                    throw (new Exception("#NotLog#找不到对应流程任务！"));
                }
                if (taskEntity.State != 1)
                {
                    throw (new Exception("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(taskEntity.ProcessId.ToString(), async _ =>
                {
                    var iWFEngine = await Bootstraper("", taskEntity.ProcessId, null, null, null, string.Empty, null);
                    // 1.判断任务类型 1 普通任务 5 会签任务
                    WorkFlowProcess processEntity2 = await _processRepository.GetAsync(taskEntity.ProcessId);
                    processEntity2.IsStart = 1;
                    processEntity2.IsCancel = 0;
                    await _processRepository.UpdateAsync(processEntity2);

                    // 更新上一个流转过来的任务，提示他无法被撤销（放在这，防止锁表）
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntity.ProcessId, taskEntity.PrevTaskId);

                    if (taskEntity.Type == 1 || taskEntity.Type == 3)
                    {
                        // 关闭同一个节点，其他人的任务 （放在这，防止锁表）
                        await _workFlowTaskSerivce.CloseTask3(taskEntity.ProcessId, taskEntity.Token, taskEntity.Id);

                        // 更新上一个流转过来的任务，提示他无法被撤销（放在这，防止锁表）
                        if (iWFEngine.Config.Params.IsChild || iWFEngine.Config.Params.ParentProcessId.HasValue)
                        {
                            await _workFlowTaskLogAppService.CloseTaskLogCancel(iWFEngine.Config.Params.ParentProcessId.Value, taskEntity.PrevTaskId);
                        }
                    }
                    else if (taskEntity.Type != 5)
                    {
                        throw (new UserFriendlyException($"#NotLog#该任务无法审批,任务类型为【{taskEntity.Type}】！"));
                    }

                    await _messageAppService.VirtualDeleteByContentId(taskEntity.Token); // 更新消息

                    if (iWFEngine.Config.Params.IsChild)
                    {
                        var processEntity = await _processRepository.GetAsync(iWFEngine.Config.Params.ParentProcessId.Value);
                        processEntity.IsStart = 1;
                        processEntity.IsCancel = 0;
                        await _processRepository.UpdateAsync(processEntity);
                    }
                });
                return new JsonResult(new
                {
                    Message = L("流程审批成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
        #endregion

        #region 加签流程
        /// <summary>
        /// 加签-process/sign/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        public async Task<IActionResult> SignFlow(SignFlowInput input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.TaskId);
                if (task == null)
                {
                    throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                }
                else if (task.State != 1)
                {
                    throw (new UserFriendlyException("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    // 更新任务状态
                    task.State = 5;
                    await _taskRepository.UpdateAsync(task);
                    // 更新上一个流转过来的任务，提示他无法被撤销
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, task.PrevTaskId);

                    var processEntity = await _processRepository.GetAsync(task.ProcessId);
                    if (processEntity.IsChild == 1 && processEntity.ParentProcessId.HasValue)
                    {
                        await _workFlowTaskLogAppService.CloseTaskLogCancel(processEntity.ParentProcessId.Value, task.PrevTaskId);
                    }
                    var taskDto = ObjectMapper.Map<WorkFlowTaskDto>(task);
                    // 填写日志
                    await CreateTaskLog(taskDto, "sign", "加签", input.Des, "", input.FileId, input.Tag);
                    if (task.Type != 6)
                    {
                        task.FirstId = task.Id;
                    }
                    List<Guid> toUserIds = input.ToUserId.Split(',')
                             .Select(Guid.Parse)
                             .ToList();
                    var userList = await _userAppService.GetListByKeyValues(toUserIds);
                    var num = 1;
                    //加签策略
                    if (input.SignType == null)
                    {
                        input.SignType = 1;
                    }
                    foreach (var userInfo in userList)
                    {
                        var newTask = task.ToJson().ToObject<WorkFlowTask>();
                        newTask.Id = _guidGenerator.Create();
                        // 添加加签任务
                        newTask.PrevTaskId = task.Id.ToString();
                        newTask.UserCompanyId = userInfo.CompanyId;
                        newTask.UserDepartmentId = userInfo.DepartmentId;
                        newTask.UserId = userInfo.Id;
                        newTask.UserName = userInfo.Name;
                        newTask.Type = 6;
                        newTask.State = 2;
                        newTask.ReadStatus = 0;
                        if ((input.SignType == 1 && num == 1) || input.SignType == 2)
                        {
                            newTask.State = 1;
                        }
                        newTask.Sort = num;
                        newTask.CreatorUserName = await UserManager.GetUserNameAsync(ObjectMapper.Map<User>(userInfo));
                        newTask.IsLast = 1;
                        num++;
                        await _taskRepository.InsertAsync(newTask);
                    }
                });
                // 更新消息
                await _messageAppService.VirtualDeleteByContentId(task.Token);
                return new JsonResult(new
                {
                    Message = L("流程审批成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("加签失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 加签审批-process/signaudit/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        public async Task<IActionResult> SignAudit(SignAuditInput input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.TaskId);
                if (task == null)
                {
                    throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                }
                if (task.State != 1)
                {
                    throw (new UserFriendlyException("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    var myTaskEntiy = await _taskRepository.GetAsync(task.FirstId); // 原始审批任务
                    myTaskEntiy.IsLast = 0;
                    await _taskRepository.UpdateAsync(myTaskEntiy);
                    var myTaskEntiy1 = myTaskEntiy.ToObject<WorkFlowTask>();
                    myTaskEntiy1.Id = _guidGenerator.Create();
                    myTaskEntiy1.State = 1;
                    myTaskEntiy1.IsLast = 1;
                    await _taskRepository.InsertAsync(myTaskEntiy1);


                    // 更新任务状态
                    task.State = 3;
                    await _taskRepository.UpdateAsync(task);

                    var taskDto = ObjectMapper.Map<WorkFlowTaskDto>(task);
                    // 填写日志
                    await CreateTaskLog(taskDto, input.Code, input.Name, input.Des, "", input.FileId, input.Tag);

                    // 更新上一个流转过来的任务，提示他无法被撤销 
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, task.PrevTaskId);


                    await _messageAppService.VirtualDeleteByContentId(task.Token); // 更新消息
                });
                return new JsonResult(new
                {
                    Message = L("审批成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("加签失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 加签审批（暂存）--process/savesignaudit/{id}")
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<IActionResult> SaveSignAudit(EntityDto<Guid> input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.Id);
                if (task == null)
                {
                    throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                }
                else if (task.State != 1)
                {
                    throw (new UserFriendlyException("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    // 更新上一个流转过来的任务，提示他无法被撤销
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, task.PrevTaskId);
                });

                await _messageAppService.VirtualDeleteByContentId(task.Token); // 更新消息
                return new JsonResult(new
                {
                    Message = L("审批成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("审批失败：" + ex.Message);
            }
        }

        #endregion 

        #region 沟通流程


        /// <summary>
        /// 沟通--process/connect/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        public async Task<IActionResult> ConnectWorkFlow(ConnectWorkFlowInput input)
        {
            try
            {
                var taskEntiy = await _taskRepository.GetAsync(input.TaskId);
                if (taskEntiy == null)
                {
                    throw (new Exception("#NotLog#找不到对应流程任务！"));
                }

                if (taskEntiy.State != 1)
                {
                    throw (new Exception("#NotLog#该任务未被激活！"));
                }
                // 更新任务状态
                taskEntiy.State = 9;
                await _taskRepository.UpdateAsync(taskEntiy);

                // 更新上一个流转过来的任务，提示他无法被撤销
                await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                if (processEntity.IsChild == 1 && processEntity.ParentProcessId.HasValue)
                {
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(processEntity.ParentProcessId.Value, taskEntiy.PrevTaskId);
                }
                // 填写日志
                await CreateTaskLog(ObjectMapper.Map<WorkFlowTaskDto>(taskEntiy), "connect", "沟通", input.Des, "", input.FileId, input.Tag);
                taskEntiy.FirstId = taskEntiy.Id;
                List<Guid> toUserIds = input.UserIds.Split(',').Select(Guid.Parse).ToList();
                var userList = await _userAppService.GetListByKeyValues(toUserIds);

                foreach (var userInfo in userList)
                {
                    var newTask = taskEntiy.ToJson().ToObject<WorkFlowTask>();
                    // 添加沟通任务
                    newTask.PrevTaskId = taskEntiy.Id.ToString();
                    newTask.UserCompanyId = userInfo.CompanyId;
                    newTask.UserDepartmentId = userInfo.DepartmentId;
                    newTask.UserId = userInfo.Id;
                    newTask.UserName = userInfo.Name;
                    newTask.Type = 7;
                    newTask.State = 1;
                    newTask.ReadStatus = 0;
                    newTask.Sort = input.ConnectType; // 用来保存沟通类型
                    newTask.CreatorUserName = await UserManager.GetUserNameAsync(ObjectMapper.Map<User>(userInfo));
                    newTask.IsLast = 1;
                    await _taskRepository.InsertAsync(newTask);
                }
                await _messageAppService.VirtualDeleteByContentId(taskEntiy.Token); // 更新消息
                return new JsonResult(new
                {
                    Message = L("沟通成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("审批失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 取消流程沟通审批--process/cancelConnect/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<IActionResult> CancelConnectAudit(EntityDto<Guid> input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.Id);
                if (task == null)
                {
                    throw (new Exception("#NotLog#找不到对应流程任务！"));
                }
                if (task.State != 9)
                {
                    throw (new Exception("#NotLog#该任务不在沟通中！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    var list = await GetUnFinishedConnectALLTaskList(task.Id);
                    foreach (var item in list)
                    {
                        item.State = 4;
                        await _taskRepository.UpdateAsync(item);
                    }

                    // 更新任务状态
                    task.State = 1;
                    await _taskRepository.UpdateAsync(task);

                    // 填写日志
                    await CreateTaskLog(ObjectMapper.Map<WorkFlowTaskDto>(task), "cancelConnect", "取消沟通", "", "", "", "");

                    // 更新消息
                    await _messageAppService.VirtualDeleteByContentId(task.Token);

                });
                return new JsonResult(new
                {
                    Message = L("沟通成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("审批失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 沟通审批--process/connectaudit/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        public async Task<IActionResult> ConnectAudit(ConnectAuditInput input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.TaskId);
                if (task == null)
                {
                    throw (new Exception("#NotLog#找不到对应流程任务！"));
                }
                if (task.State == 9)
                {
                    throw (new Exception("#NotLog#还有人没有回复,无法完成任务！"));
                }
                if (task.State != 1)
                {
                    throw (new Exception("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    var connectTaskList = await _workFlowTaskSerivce.GetUnFinishedConnectTaskList(task.FirstId);
                    if (connectTaskList.Count() == 1 && connectTaskList.FirstOrDefault().Id == input.TaskId)
                    {
                        var myTaskEntiy = await _taskRepository.GetAsync(task.FirstId); // 原始审批任务
                        myTaskEntiy.State = 1;
                        await _taskRepository.UpdateAsync(myTaskEntiy);
                    }
                    // 更新任务状态
                    task.State = 3;
                    await _taskRepository.UpdateAsync(task);

                    // 填写日志
                    await CreateTaskLog(ObjectMapper.Map<WorkFlowTaskDto>(task), input.Code, input.Name, input.Des, "", input.FileId, input.Tag);

                    // 更新上一个流转过来的任务，提示他无法被撤销
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, task.PrevTaskId);

                    // 更新消息
                    await _messageAppService.VirtualDeleteByContentId(task.Token);
                });
                return new JsonResult(new
                {
                    Message = L("沟通成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("审批失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 沟通审批（暂存）--process/saveconnectaudit/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        public async Task<IActionResult> SaveConnectAudit(EntityDto<Guid> input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.Id);
                if (task == null)
                {
                    throw (new Exception("#NotLog#找不到对应流程任务！"));
                }
                if (task.State != 1)
                {
                    throw (new Exception("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    // 更新上一个流转过来的任务，提示他无法被撤销
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(task.ProcessId, task.PrevTaskId);
                });
                await _messageAppService.VirtualDeleteByContentId(task.Token); // 更新消息
                return new JsonResult(new
                {
                    Message = L("沟通成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("审批失败：" + ex.Message);
            }
        }

        private async Task<List<WorkFlowTask>> GetUnFinishedConnectALLTaskList(Guid taskId)
        {
            var res = new List<WorkFlowTask>();
            var list = await _workFlowTaskSerivce.GetUnFinishedConnectTaskList(taskId);
            res.AddRange(list);
            foreach (var item in list)
            {
                res.AddRange(await GetUnFinishedConnectALLTaskList(item.Id));
            }

            return res;
        }

        #endregion

        #region 传阅
        /// <summary>
        /// 传阅分享--process/share/{id}
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        public async Task<IActionResult> ShareWorkFlow(ShareWorkFlowInput input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.TaskId);
                if (task == null)
                {
                    throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                }
                await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                {
                    // 填写日志
                    await CreateTaskLog(ObjectMapper.Map<WorkFlowTaskDto>(task), "share", "传阅", "", "", "", "");
                    task.FirstId = task.Id;
                    var userList = input.ToUserId.Split(",");
                    foreach (var userId in userList)
                    {
                        var userInfo = await UserManager.GetUserByIdAsync(Guid.Parse(userId));
                        task.Id = _guidGenerator.Create();
                        // 添加传阅查看任务
                        task.PrevTaskId = task.Id.ToString();
                        task.UserCompanyId = userInfo.CompanyId;
                        task.UserDepartmentId = userInfo.DepartmentId;
                        task.UserId = userInfo.Id;
                        task.UserName = userInfo.Name;
                        task.Type = 2;
                        task.State = 1;
                        await _taskRepository.InsertAsync(task);
                    }
                });

                await _messageAppService.VirtualDeleteByContentId(task.Token); // 更新消息
                return new JsonResult(new
                {
                    Message = L("传阅成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("传阅失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 确认阅读-process/read/{id}
        /// </summary>
        /// <param name="id">流程任务主键</param>
        /// <returns></returns>
        public async Task<IActionResult> ReadWorkFlow(EntityDto<Guid> input)
        {
            try
            {
                var task = await _taskRepository.GetAsync(input.Id);
                if (task == null)
                {
                    throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
                }
                else if (task.State == 1)
                {
                    await LockExtensions.Locking(task.ProcessId.ToString(), async _ =>
                    {
                        task.State = 3;
                        await _taskRepository.UpdateAsync(task);

                        WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
                        {
                            TaskType = task.Type,
                            ProcessId = task.ProcessId,
                            UnitId = task.UnitId,
                            UnitName = task.UnitName,
                            IsAgree = task.IsAgree,
                            OperationCode = "read",
                            OperationName = "已阅",
                            Token = task.Token,
                            PrevUnitId = task.PrevUnitId,
                            PrevUnitName = task.PrevUnitName,
                            TaskId = task.Id,
                            IsCancel = 0
                        };
                        await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);

                    });
                }
                // 更新消息
                await _messageAppService.VirtualDeleteByContentId(task.Token);

                return new JsonResult(new
                {
                    Message = L("确认成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("传阅失败：" + ex.Message);
            }
        }


        #endregion

        #region 流程转办
        /// <summary>
        /// 转办审批人-process/transfer/{id}
        /// </summary>
        /// <param name="id">流程任务主键</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        public async Task<IActionResult> TransferUser(TransferUserInput input)
        {
            try
            {
                var taskEntiy = await _taskRepository.GetAsync(input.TaskId);
                if (taskEntiy == null)
                {
                    throw (new Exception("#NotLog#找不到对应流程任务！"));
                }
                else if (taskEntiy.State != 1)
                {
                    throw (new Exception("#NotLog#该任务未被激活！"));
                }
                await LockExtensions.Locking(taskEntiy.ProcessId.ToString(), async _ =>
                {
                    var userInfo = await UserManager.GetUserByIdAsync(input.ToUserId);
                    // 更新任务状态
                    taskEntiy.State = 6;
                    await _taskRepository.UpdateAsync(taskEntiy);

                    // 更新流程
                    var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                    processEntity.IsStart = 1;
                    processEntity.IsCancel = 0;
                    await _processRepository.UpdateAsync(processEntity);


                    // 填写日志
                    await CreateTaskLog(ObjectMapper.Map<WorkFlowTaskDto>(taskEntiy), "transfer", "转办", input.Des, "", input.FileId, input.Tag);
                    // 更新上一个流转过来的任务，提示他无法被撤销
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                    if (processEntity.IsChild == 1 && processEntity.ParentProcessId.HasValue)
                    {
                        await _workFlowTaskLogAppService.CloseTaskLogCancel(processEntity.ParentProcessId.Value, taskEntiy.PrevTaskId);
                    }

                    // 新建任务
                    var newTask = taskEntiy.ToJson().ToObject<WorkFlowTask>();
                    newTask.State = 1;
                    newTask.PrevTaskId = taskEntiy.Id.ToString();
                    newTask.UserCompanyId = userInfo.CompanyId;
                    newTask.UserDepartmentId = userInfo.DepartmentId;
                    newTask.UserId = userInfo.Id;
                    newTask.UserName = userInfo.Name;
                    newTask.Id = _guidGenerator.Create();

                    await _taskRepository.InsertAsync(newTask);

                    // 发送消息
                    var iWFEngine = await Bootstraper("", taskEntiy.ProcessId, null, null, null, string.Empty, null);
                    var msg = $"【审批】{taskEntiy.ProcessTitle},{taskEntiy.UnitName}";
                    var userList = new List<Guid>();
                    userList.Add(userInfo.Id);
                    var node = iWFEngine.GetNode(taskEntiy.UnitId);
                    await _messageAppService.SendMsg(new Massages.Dto.SendMassageDto()
                    {
                        UserIdList = userList,
                        MessageType = node.MessageType,
                        ContentId = newTask.Token,
                        Content = msg,
                        Code = "IMWF"
                    });
                });
                await _messageAppService.VirtualDeleteByContentId(taskEntiy.Token); // 更新消息
                return new JsonResult(new
                {
                    Message = L("转办成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("转办失败：" + ex.Message);

            }
        }

        #endregion

        #region 流程操作
        /// <summary>
        /// 作废流程-process/id
        /// </summary>
        /// <param name="id">流程进程主键</param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteProcess(EntityDto<Guid> input)
        {
            try
            {
                await LockExtensions.Locking(input.Id.ToString(), async _ =>
                {
                    var iWFEngine = await Bootstraper(string.Empty, input.Id, null, null, null, string.Empty, null);
                    // 判断是否有子流程
                    var childList = await this.GetChildList(input.Id);
                    foreach (var childItem in childList)
                    {
                        await DeleteProcess(new EntityDto<Guid>() { Id = childItem.Id });
                    }

                    // 更新流程状态
                    WorkFlowProcess processEntity = await _processRepository.GetAsync(input.Id);

                    processEntity.IsActive = 3;

                    await _processRepository.UpdateAsync(processEntity);

                    await _workFlowTaskSerivce.VirtualDelete(input.Id);
                    await ExecuteScript(input.Id, "delete", "", iWFEngine.CreateUser,
                        iWFEngine.WFScheme.DeleteType,
                        iWFEngine.WFScheme.DeleteDbCode,
                        iWFEngine.WFScheme.DeleteDbSQL,
                        iWFEngine.WFScheme.DeleteUrl,
                        iWFEngine.WFScheme.DeleteIOCName);
                });
                return new JsonResult(new
                {
                    Message = L("作废成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("作废失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 彻底删除流程-process/remove/id
        /// </summary>
        /// <param name="id">流程进程主键</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Monitor_Delete)]
        public async Task<IActionResult> RemoveProcess(EntityDto<Guid> input)
        {
            try
            {
                await LockExtensions.Locking(input.Id.ToString(), async _ =>
                {
                    var iWFEngine = await Bootstraper(string.Empty, input.Id, null, null, null, string.Empty, null);
                    // 判断是否有子流程
                    var childList = await this.GetChildList(input.Id);
                    foreach (var childItem in childList)
                    {
                        await RemoveProcess(new EntityDto<Guid>() { Id = childItem.Id });
                    }
                    await _processRepository.DeleteAsync(input.Id);
                    await _taskRepository.DeleteAsync(x => x.ProcessId.Equals(input.Id));
                    await _workFlowTaskLogAppService.DeleteLogByProcessId(input.Id);
                    await ExecuteScript(input.Id, "remove", "", iWFEngine.CreateUser,
                        iWFEngine.WFScheme.DeleteType,
                        iWFEngine.WFScheme.DeleteDbCode,
                        iWFEngine.WFScheme.DeleteDbSQL,
                        iWFEngine.WFScheme.DeleteUrl,
                        iWFEngine.WFScheme.DeleteIOCName);
                });
                return new JsonResult(new
                {
                    Message = L("删除成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("删除失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 指派流程审批人-process/point/id
        /// </summary>
        /// <param name="id">流程任务主键</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Monitor_UpdateWorkFlowUser)]
        public async System.Threading.Tasks.Task UpdateWorkFlowUser(UpdateWorkFlowUserInput input)
        {
            try
            {
                await LockExtensions.Locking(input.ProcessId.ToString(), async _ =>
                {
                    var iWFEngine = await Bootstraper("", input.ProcessId, null, null, null, string.Empty, null);
                    var task =await _taskRepository.FirstOrDefaultAsync(x => x.Id.Equals(input.TaskId));
                    foreach (var userId in input.UserIds.Split(','))
                    {
                        var addItem = task.ToJson().ToObject<WorkFlowTask>();
                        addItem.Id = _guidGenerator.Create();
                        addItem.State = 1;
                        if (addItem.Type == 6)
                        {
                            addItem.Type = 1;
                        }
                        var userEntity = await UserManager.GetUserByIdAsync(Guid.Parse(userId));
                        addItem.UserId = userEntity.Id;
                        addItem.UserName = userEntity.Name;
                        addItem.UserCompanyId = userEntity.CompanyId;
                        addItem.UserDepartmentId = userEntity.DepartmentId;


                        // 发送消息
                        var msg = $"【审批】{addItem.ProcessTitle},{addItem.UnitName}";
                        var userList = new List<Guid>();
                        userList.Add(Guid.Parse(userId));
                        var node = iWFEngine.GetNode(addItem.UnitId);
                        await _messageAppService.SendMsg(new Massages.Dto.SendMassageDto()
                        {
                            Code = "IMWF",
                            UserIdList = userList,
                            Content = msg,
                            MessageType = node.MessageType,
                            ContentId = addItem.Token

                        });
                        await _taskRepository.InsertAsync(addItem);
                    }

                    task.Type = 4;//关闭任务
                    await _taskRepository.UpdateAsync(task);
                });
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("指派失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 指派流程审批人-process/point/id
        /// </summary>
        /// <param name="id">流程任务主键</param>
        /// <param name="dto">流程信息</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Monitor_PointUser)]
        public async System.Threading.Tasks.Task PointWorkFlowUser(PointWorkFlowUserInput input)
        {
            try
            {
                await LockExtensions.Locking(input.ProcessId.ToString(), async _ =>
                {
                    var list = await _workFlowTaskSerivce.GetUnFinishTaskList(input.ProcessId);
                    Dictionary<Guid, List<WorkFlowTaskDto>> map = new Dictionary<Guid, List<WorkFlowTaskDto>>();
                    foreach (var item in list)
                    {
                        if (item.State != 8)
                        {
                            if (!map.ContainsKey(item.UnitId))
                            {
                                map.Add(item.UnitId, new List<WorkFlowTaskDto>());
                            }
                            map[item.UnitId].Add(item);
                        }
                    }
                    //var addList = new List<WorkFlowTask>();
                    var iWFEngine = await Bootstraper("", input.ProcessId, null, null, null, string.Empty, null);
                    foreach (var item in input.PointList)
                    {
                        if (map.ContainsKey(item.UnitId))
                        {
                            await _workFlowTaskSerivce.CloseTask3(input.ProcessId, map[item.UnitId][0].Token.ToString(), null);
                            var userIdList = item.UserIds.Split(",");
                            foreach (var userId in userIdList)
                            {
                                var addItem = map[item.UnitId][0].ToJson().ToObject<WorkFlowTask>();
                                addItem.Id = _guidGenerator.Create();
                                addItem.State = 1;
                                if (addItem.Type == 6)
                                {
                                    addItem.Type = 1;
                                }

                                var userEntity = await UserManager.GetUserByIdAsync(Guid.Parse(userId));

                                addItem.UserId = userEntity.Id;
                                addItem.UserName = userEntity.Name;
                                addItem.UserCompanyId = userEntity.CompanyId;
                                addItem.UserDepartmentId = userEntity.DepartmentId;

                                // 发送消息
                                var msg = $"【审批】{addItem.ProcessTitle},{addItem.UnitName}";
                                var userList = new List<Guid>();
                                userList.Add(Guid.Parse(userId));
                                var node = iWFEngine.GetNode(addItem.UnitId);
                                await _messageAppService.SendMsg(new Massages.Dto.SendMassageDto()
                                {
                                    Code = "IMWF",
                                    UserIdList = userList,
                                    Content = msg,
                                    MessageType = node.MessageType,
                                    ContentId = addItem.Token

                                });
                                //addList.Add(addItem);
                                await _taskRepository.InsertAsync(addItem);
                            }
                        }
                        else
                        {
                            throw (new Exception("#NotLog#找不到指派任务节点！"));
                        }
                    }
                    //foreach (var item in addList)
                    //{
                    //    await _taskRepository.InsertAsync(item);
                    //}
                });
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException("指派失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 修改流程配置信息-process/scheme//{id}
        /// </summary>
        /// <param name="id">流程任务主键</param>
        /// <param name="dto">模版信息</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Monitor_UpdateProcessScheme)]
        public async Task<IActionResult> UpdateProcessScheme(UpdateProcessInput input)
        {
            try
            {
                var schemeEntity = new WorkFlowScheme();
                schemeEntity.Content = input.Schema;
                if (string.IsNullOrEmpty(schemeEntity.Content))
                {
                    return new JsonResult(new
                    {
                        Message = L("流程模板为空"),
                        Success = false
                    });
                }
                schemeEntity.Content = AESHelper.AesDecrypt(schemeEntity.Content, "ADTODCloud");
                var processEntity = await _processRepository.GetAsync(input.Id);
                var old = await _schemeRepository.GetAsync(processEntity.SchemeId.Value);

                if (old.Content == schemeEntity.Content)
                {
                    return new JsonResult(new
                    {
                        Message = L("流程模板与历史模板内容一致"),
                        Success = false
                    });
                }

                schemeEntity.SchemeInfoId = input.Id;// 
                schemeEntity.Type = 3; // 流程实例临时修改模版
                var id = await _schemeRepository.InsertAndGetIdAsync(schemeEntity);
                processEntity.SchemeId = id;
                await _processRepository.UpdateAsync(processEntity);
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

        /// <summary>
        /// 流转流程节点-process/transferNode
        /// </summary>
        /// <param name="id">流程实例id</param>
        /// <param name="startId">开始节点</param>
        /// <param name="endId">结束节点</param>
        /// <returns></returns>
        public async Task<IActionResult> TransferNode(TransferNodeInput input)
        {
            try
            {
                await LockExtensions.Locking(input.Id.ToString(), async _ =>
                {
                    var iWFEngine = await Bootstraper("", input.Id, null, null, null, string.Empty, null);
                    if (iWFEngine.Config.Params.State == 2)
                    {
                        throw (new Exception("#NotLog#当前流程任务已经结束！"));
                    }
                    if (iWFEngine.Config.Params.State == 4)
                    {
                        throw (new Exception("#NotLog#当前流程任务已经作废！"));
                    }
                    var taskNode = iWFEngine.GetNode(input.EndId);
                    List<WorkFlowUnit> taskNodes = new List<WorkFlowUnit>();
                    taskNodes.Add(taskNode);
                    var taskList = await iWFEngine.GetTask1(taskNodes, false, input.StartId, 0, false);
                    if (taskList.FindAll(t => t.Type != 10).Count == 0)
                    {
                        throw (new Exception("#NotLog#找不到下一个流转节点！"));
                    }
                    WorkFlowProcess processEntity = await _processRepository.GetAsync(input.Id);
                    processEntity.IsStart = 1;
                    processEntity.IsCancel = 0;

                    List<WorkFlowTaskDto> myTaskList;
                    // 更新流程状态
                    if (taskList.FindIndex(t => t.Type == 100) != -1)
                    {
                        processEntity.IsFinished = 1;
                        processEntity.FinishTime = DateTime.Now;
                        processEntity.ProcessMinutes = (int)((DateTime)processEntity.FinishTime - (DateTime)processEntity.CreationTime).TotalMinutes;
                    }
                    else if (taskList.FindIndex(t => t.Type == 4) != -1)
                    {
                        if (iWFEngine.Config.Params.IsChild)
                        {
                            processEntity.IsFinished = 1;
                            processEntity.FinishTime = DateTime.Now;
                            processEntity.ProcessMinutes = (int)((DateTime)processEntity.FinishTime - (DateTime)processEntity.CreationTime).TotalMinutes;
                        }
                        processEntity.IsAgain = 1;
                    }
                    await _processRepository.UpdateAsync(processEntity);

                    // 更新任务状态
                    var taskEntity = await _workFlowTaskSerivce.GetEntityByUnitId(input.Id, input.StartId);
                    taskEntity.IsAgree = 1;
                    taskEntity.State = 4;
                    await _workFlowTaskSerivce.UpdateAsync(taskEntity);

                    // 更新上一个流转过来的任务，提示他无法被撤销
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntity.ProcessId, taskEntity.PrevTaskId);
                    if (iWFEngine.Config.Params.IsChild && iWFEngine.Config.Params.ParentProcessId.HasValue)
                    {
                        await _workFlowTaskLogAppService.CloseTaskLogCancel(iWFEngine.Config.Params.ParentProcessId.Value, taskEntity.PrevTaskId);
                    }
                    // 关闭同一个节点，其他人的任务
                    await _workFlowTaskSerivce.CloseTask3(taskEntity.ProcessId, taskEntity.Token, taskEntity.Id);

                    myTaskList = await ExecuteWFTaskEntity(taskList, iWFEngine, taskEntity.ProcessId, taskEntity.Token, 0, taskEntity.Id.ToString());
                    foreach (var item in myTaskList)
                    {
                        var task = ObjectMapper.Map<WorkFlowTask>(item);
                        await _taskRepository.InsertAsync(task);
                    }
                    await CreateTaskLog(taskEntity, "TransferNode", "转移节点", "", "", "", "", true);
                    // 子流程创建
                    var autoChildList = myTaskList.FindAll(t => t.Type == 3 && t.IsAuto == 1);
                    WorkFlowProcess parentProcessEntity = null;
                    foreach (var childItem in autoChildList)
                    {
                        // 自动创建子流程或者再发起子流程
                        var node = iWFEngine.GetNode(childItem.UnitId);
                        var userInfo = await UserManager.GetUserByIdAsync(childItem.UserId.Value);
                        var childIWFEngine = await Bootstraper(string.Empty, childItem.ChildProcessId ?? Guid.Empty, node.WfVersionId, childItem.ProcessId, childItem.Id, "", userInfo, null);

                        if (parentProcessEntity == null)
                        {
                            parentProcessEntity = await _processRepository.GetAsync(processEntity.Id);
                        }

                        await this.CreateFlow(childIWFEngine, null, "", "", parentProcessEntity.Level, parentProcessEntity.SecretLevel, childItem.Token, childItem.Id.ToString());
                        // 更新任务状态
                        childItem.IsAgree = 1;
                        childItem.State = 8;
                        await _workFlowTaskSerivce.UpdateAsync(childItem);
                        await CreateTaskLog(childItem, "createsub", "子流程提交", "", "", "", "");

                    }
                    // 如果流程结束且是子流程需要跳转到父级流程
                    if (iWFEngine.Config.Params.IsChild)
                    {
                        if (processEntity.IsAgain == 1)
                        {

                            if (await AgainToParent(iWFEngine.Config.Params.ParentTaskId.Value))
                            {
                                var againTask = myTaskList.Find(t => t.Type == 4);
                                againTask.State = 3;
                                await _workFlowTaskSerivce.UpdateAsync(againTask);
                            }
                        }
                        else
                        {
                            if (processEntity.IsFinished == 1)
                            {
                                await FinishToParent(iWFEngine.Config.Params.ParentTaskId);
                            }
                        }
                    }
                });
                return new JsonResult(new
                {
                    Message = L("流转成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("流转失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 流程归档--process/placeonfile/id
        /// </summary>
        /// <param name="id">流程实例id</param>
        /// <returns></returns>
        [ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Monitor_PlaceOnFile)]
        public async Task<IActionResult> PlaceOnFile(EntityDto<Guid> input)
        {
            try
            {
                await LockExtensions.Locking(input.Id.ToString(), async _ =>
                {
                    var iWFEngine = await Bootstraper("", input.Id, null, null, null, string.Empty, null);
                    if (iWFEngine.Config.Params.State == 2)
                    {
                        throw (new Exception("#NotLog#当前流程任务已经结束！"));
                    }
                    if (iWFEngine.Config.Params.State == 4)
                    {
                        throw (new Exception("#NotLog#当前流程任务已经作废！"));
                    }
                    WorkFlowProcess processEntity = await _processRepository.GetAsync(input.Id);
                    processEntity.IsStart = 1;
                    processEntity.IsCancel = 0;
                    processEntity.IsFinished = 1;
                    processEntity.FinishTime = DateTime.Now;
                    processEntity.ProcessMinutes = (int)((DateTime)processEntity.FinishTime - (DateTime)processEntity.CreationTime).TotalMinutes;


                    // 处理任务并更新数据

                    await _processRepository.UpdateAsync(processEntity);

                    List<WorkFlowUnit> taskNodes = new List<WorkFlowUnit>();
                    taskNodes.Add(iWFEngine.EndNode);
                    var taskList = await iWFEngine.GetTask1(taskNodes, false, iWFEngine.StartNode.Id, 0, false);

                    await ExecuteWFTaskEntity(taskList, iWFEngine, input.Id, "", 0, "");

                    // 关闭所有任务
                    await _workFlowTaskSerivce.CloseTaskByProcessId(input.Id);

                    // 记录日志
                    WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
                    {
                        Remark = "流程归档",
                        TaskType = 200,
                        ProcessId = input.Id,
                        UnitId = iWFEngine.EndNode.Id,
                        UnitName = string.IsNullOrEmpty(iWFEngine.EndNode.Name) ? "结束节点" : iWFEngine.EndNode.Name
                    };
                    await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);

                    // 如果流程结束且是子流程需要跳转到父级流程
                    if (iWFEngine.Config.Params.IsChild)
                    {
                        await FinishToParent(iWFEngine.Config.Params.ParentTaskId);
                    }

                });
                return new JsonResult(new
                {
                    Message = L("归档成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("归档失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 设置任务置顶-process/taskup/{id}/{isUp}
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="isUp">是否置顶</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateTaskUpStatus(Guid id, int isUp)
        {
            try
            {
                var task = await _taskRepository.GetAsync(id);
                task.IsUp = isUp;
                await _taskRepository.UpdateAsync(task);
                return new JsonResult(new
                {
                    Message = L("操作成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("操作失败：" + ex.Message);
            }
        }
        /// <summary>
        /// 设置任务置顶（已办）-process/taskup2/{id}/{isUp}
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <param name="isUp">是否置顶</param>
        /// <returns></returns>
        //[HttpPost("workflow/process/taskup2/{id}/{isUp}")]
        [HttpPost]
        public async Task<IActionResult> UpdateTaskUpStatus2(Guid id, int isUp)
        {
            try
            {
                var task = await _taskRepository.GetAsync(id);
                task.IsUp2 = isUp;
                await _taskRepository.UpdateAsync(task);
                return new JsonResult(new
                {
                    Message = L("操作成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("操作失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 催办流程-process/urge/{id
        /// </summary>
        /// <param name="id">流程进程主键</param>
        [HttpPost]
        public async Task<IActionResult> UrgeFlow(Guid processId)
        {
            try
            {
                var iWFEngine = await Bootstraper(string.Empty, processId, null, null, null, null, null);
                // 获取未完成的任务
                var taskList = await _workFlowTaskSerivce.GetUnFinishTaskList(processId);
                foreach (var item in taskList)
                {
                    if (item.Type == 1 || item.Type == 3 || item.Type == 5 || item.Type == 6)
                    {
                        item.IsUrge = 1;
                        item.UrgeTime = DateTime.Now;
                        // 发送消息
                        var msg = $"【审批】【催办】{item.ProcessTitle},{item.UnitName}";
                        var userList = new List<Guid>();
                        userList.Add(item.UserId.Value);
                        var node = iWFEngine.GetNode(item.UnitId);
                        await _messageAppService.SendMsg(new Massages.Dto.SendMassageDto()
                        {
                            Code = "IMWF",
                            UserIdList = userList,
                            Content = msg,
                            MessageType = node.MessageType,
                            ContentId = item.Token

                        });
                        await _workFlowTaskSerivce.UpdateAsync(item);
                        WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
                        {
                            Remark = "催办审批",
                            TaskType = 98,
                            ProcessId = processId,
                            UnitId = iWFEngine.StartNode.Id,
                            UnitName = string.IsNullOrEmpty(iWFEngine.StartNode.Name) ? "开始节点" : iWFEngine.StartNode.Name
                        };
                        await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);
                    }
                }
                return new JsonResult(new
                {
                    Message = L("操作成功"),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("操作失败：" + ex.Message);
            }
        }

        #endregion

        #region 流程自定义标题
        /// <summary>
        /// 获取流程自定义标题
        /// </summary>
        /// <param name="wFSchemeInfo"></param>
        /// <returns></returns>
        public async Task<string> GetCustmerTitle(Guid processId, WorkFlowSchemeinfoDto wFSchemeInfo, User userEntity)
        {
            StringBuilder sb = new StringBuilder();
            var wFScheme = await _workFlowSchemeAppService.GetSchemeEntity(wFSchemeInfo.SchemeId);
            var _wfScheme = wFScheme.Content.ToObject<WFScheme>();
            var startEvent = _wfScheme.WfData.Find(it => it.Type == "startEvent");
            if (startEvent.TitleRule.Count > 0)
            {
                foreach (var rule in startEvent.TitleRule)
                {
                    sb.Append(await GetRuleVaule(processId, rule, wFSchemeInfo, userEntity));
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取标题自定义规则
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="titleRule"></param>
        /// <param name="wFSchemeInfo"></param>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        private async Task<string> GetRuleVaule(Guid processId, WorkFlowCustmerTitleRule titleRule, WorkFlowSchemeinfoDto wFSchemeInfo, User userEntity)
        {
            var rulevaule = string.Empty;
            switch (titleRule.type)
            {
                case "hyphen"://连接符【-/+】
                    rulevaule = titleRule.value;
                    break;
                case "serialNum"://流水号
                    rulevaule = await GetSerialNum(wFSchemeInfo.Code, titleRule.value);
                    break;
                case "randomNumber"://随机数
                    rulevaule = DtoSortingHelper.GetRndStrOrNum(titleRule.value);
                    break;
                case "text"://自定义
                    rulevaule = titleRule.value;
                    break;
                case "template"://流程模板信息
                    if (titleRule.value == "templateNo")//模板编号
                    {
                        rulevaule = wFSchemeInfo.Code;
                    }
                    else if (titleRule.value == "templateName")//模板名称
                    {
                        rulevaule = wFSchemeInfo.Name;
                    }
                    else if (titleRule.value == "templateType")//模板分类
                    {
                        rulevaule = wFSchemeInfo.Category;
                    }
                    else if (titleRule.value == "templateNode")//模板备注
                    {
                        rulevaule = wFSchemeInfo.Remark;
                    }
                    break;
                case "user"://发起人信息
                    if (titleRule.value == "account")//发起人账号
                    {
                        rulevaule = userEntity.UserName;
                    }
                    else if (titleRule.value == "name")//发起人姓名
                    {
                        rulevaule = userEntity.Name;
                    }
                    else if (titleRule.value == "dept")//发起人所属部门
                    {
                        // 组织分级,默认:0,集群:1,集团:2,公司:3,部门:4,班/组:5,如果还存在其它的,在这个上面,可以进一步细分,具体解释规则由上层应用来确定

                        if (userEntity.DepartmentId != null && userEntity.DepartmentId.Value != Guid.Empty)
                        {
                            rulevaule = userEntity.DepartmentId.ToString();
                        }
                    }
                    else if (titleRule.value == "company")//发起人所属公司
                    {
                        if (userEntity.CompanyId != null && userEntity.CompanyId != Guid.Empty)
                        {
                            rulevaule = userEntity.CompanyId.ToString();
                        }
                    }
                    break;
                case "time"://流程发起时间
                    rulevaule = DateTime.Now.ToString(titleRule.value);
                    break;
                case "db": //获取表单数据
                    rulevaule = await _formSchemeAppService.GetCustmerFormData(processId, titleRule.value);
                    break;
            }
            return rulevaule;
        }
        /// <summary>
        /// 流水号
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task<string> GetSerialNum(string code, string value)
        {
            var index = await this.GetIndexByCode(code);
            index += 1;
            var num = Convert.ToInt32(value.Replace("serialNum", ""));
            return DtoSortingHelper.NumToStr(index, num);
        }
        #endregion

        #region 执行脚本
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="code"></param>
        /// <param name="des"></param>
        /// <param name="createUser"></param>
        /// <param name="executeType"></param>
        /// <param name="sqlDb"></param>
        /// <param name="sqlStr"></param>
        /// <param name="apiUrl"></param>
        /// <param name="iocName"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task ExecuteScript(Guid processId, string code, string des, WFUserInfo createUser, string executeType, string sqlDb, string sqlStr, string apiUrl, string iocName)
        {
            var param = new
            {
                processId,
                userId = createUser.Id,
                userAccount = createUser.Account,
                companyId = createUser.CompanyId,
                departmentId = createUser.DepartmentId,
                code
            };
            switch (executeType)
            {
                case "1"://执行sql
                    if (!string.IsNullOrEmpty(sqlDb) && !string.IsNullOrEmpty(sqlStr) && !string.IsNullOrWhiteSpace(sqlStr))
                    {

                        await _dataBaseService.ExecuteSql(sqlStr, param);
                    }
                    break;
                case "2":// 接口 3
                    if (!string.IsNullOrEmpty(iocName))
                    {
                        //IWorkFlowMethod iWorkFlowMethod = IocManager.Instance.Resolve<IWorkFlowMethod>(iocName);
                        WfMethodParameter wfMethodParameter = new WfMethodParameter()
                        {
                            ProcessId = processId,
                            Code = code,
                            UserId = createUser.Id,
                            UserAccount = createUser.Account,
                            CompanyId = createUser.CompanyId,
                            DepartmentId = createUser.DepartmentId,
                            Des = des
                        };
                        await _invokeMethodHelper.InvokeByNameAsync(iocName, processId);

                        //await iWorkFlowMethod.Execute(wfMethodParameter);
                    }
                    break;
                case "3"://第三方接口 IOC
                    if (!string.IsNullOrEmpty(apiUrl) && !string.IsNullOrWhiteSpace(apiUrl))
                    {
                        if (apiUrl.IndexOf("?") == -1)
                        {
                            apiUrl = $"{apiUrl}?token={RequestPermission.GetRequestToken}";
                        }
                        else
                        {
                            apiUrl = $"{apiUrl}&token={RequestPermission.GetRequestToken}";
                        }
                        await HttpMethods.Post(apiUrl, param.ToJson());
                    }
                    break;
            }
        }


        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="task">脚本任务</param>
        /// <param name="code"></param>
        /// <param name="des"></param>
        /// <param name="processId"></param>
        /// <param name="prevTaskId"></param>
        /// <param name="createUser"></param>
        /// <param name="iWFEngine"></param>
        /// <param name="preTask"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task ExecuteScript(WorkFlowTaskDto task, string code, string des, Guid processId, Guid? prevTaskId, WFUserInfo createUser, IWorkFlowEngineAppService iWFEngine, WorkFlowTaskDto preTask)
        {
            User userInfo;
            if (preTask == null)
            {
                userInfo = new User()
                {
                    Id = createUser.Id,
                    UserName = createUser.Account,
                    CompanyId = createUser.CompanyId,
                    DepartmentId = createUser.DepartmentId
                };
            }
            else
            {
                userInfo = await this.UserManager.GetUserByIdAsync(preTask.UserId.Value);
            }

            var param = new
            {
                processId,
                userId = createUser.Id,
                userAccount = createUser.Account,
                companyId = createUser.CompanyId,
                departmentId = createUser.DepartmentId,
                userId2 = userInfo.Id,
                userAccount2 = userInfo.UserName,
                companyId2 = userInfo.CompanyId,
                departmentId2 = userInfo.DepartmentId,
                childProcessId = task.ChildProcessId,
                code
            };
            try
            {
                var node = iWFEngine.GetNode(task.UnitId);
                switch (node.ExecuteType)
                {
                    case "1"://SQL
                        if (!string.IsNullOrEmpty(node.SqlDb) && !string.IsNullOrEmpty(node.SqlStr) && !string.IsNullOrWhiteSpace(node.SqlStr))
                        {
                            await _dataBaseService.ExecuteSql(node.SqlStr, param);
                        }
                        break;
                    case "2"://接口
                             //判断这个接口是否存在，
                        if (!string.IsNullOrEmpty(node.Ioc) && IocManager.Instance.IsRegistered<IWorkFlowMethod>())
                        {
                            //解析此接口，进行调用
                            IWorkFlowMethod iWorkFlowMethod = IocManager.Instance.Resolve<IWorkFlowMethod>();
                            WfMethodParameter wfMethodParameter = new WfMethodParameter()
                            {
                                ProcessId = processId,
                                UnitName = task.UnitName,
                                TaskId = prevTaskId ?? Guid.Empty,
                                Code = code,
                                UserId = createUser.Id,
                                UserAccount = createUser.Account,
                                CompanyId = createUser.CompanyId,
                                DepartmentId = createUser.DepartmentId,
                                ChildProcessId = task.ChildProcessId.Value,
                                Des = des,
                            };
                            await iWorkFlowMethod.Execute(wfMethodParameter);
                        }
                        break;
                    case "3"://IOC
                        if (!string.IsNullOrEmpty(node.ApiUrl) && !string.IsNullOrWhiteSpace(node.ApiUrl))
                        {
                            if (node.ApiUrl.IndexOf("?") == -1)
                            {
                                node.ApiUrl = $"{node.ApiUrl}?token={RequestPermission.GetRequestToken}";
                            }
                            else
                            {
                                node.ApiUrl = $"{node.ApiUrl}&token={RequestPermission.GetRequestToken}";
                            }
                            await HttpMethods.Post(node.ApiUrl, param.ToJson());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region 执行节点函数
        /// <summary>
        /// 执行节点回调函数
        /// </summary>
        /// <param name="des"></param>
        /// <param name="processId"></param>
        /// <param name="iWFEngine"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task ExecuteInvokeMethod(WorkFlowUnit node, IWorkFlowEngineAppService iWFEngine, Guid processId, string des, string isAgree)
        {
            //node.ServiceInvokeParameters = "lrsystemdb,ProjectContracts,Id,*";//表单参数
            //node.ServerCallbackFun = "ProjectContractAppService.UpdateAuditStatus";//ADTO.DCloud.ProjectManage.ProjectContracts.
            if (!string.IsNullOrWhiteSpace(node.ServerCallbackFun))
            {
                var formData = new DataTable();
                //获取表单数据
                if (!string.IsNullOrEmpty(node.ServiceInvokeParameters))
                {
                    formData = await _formSchemeAppService.GetCustmerFormDataTable(processId, node.ServiceInvokeParameters);
                }
                await _invokeMethodHelper.InvokeByNameAsync(node.ServerCallbackFun, new object[] { isAgree, processId, des, formData.ToJson() });
            }
        }
        #endregion

        #region 创建日志
        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="taskEntiy"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="des"></param>
        /// <param name="stampImg"></param>
        /// <param name="fileId"></param>
        /// <param name="tag"></param>
        /// <param name="isNotCancel"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task CreateTaskLog(WorkFlowTaskDto taskEntiy, string code, string name, string des, string stampImg, string fileId, string tag, bool isNotCancel = false)
        {
            WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
            {
                Remark = des,
                TaskType = taskEntiy.Type,
                ProcessId = taskEntiy.ProcessId,
                ChildProcessId = taskEntiy.ChildProcessId,
                UnitId = taskEntiy.UnitId,
                UnitName = taskEntiy.UnitName,
                IsAgree = taskEntiy.IsAgree,
                OperationCode = code,
                OperationName = name,
                Token = taskEntiy.Token,
                PrevUnitId = taskEntiy.PrevUnitId,
                PrevUnitName = taskEntiy.PrevUnitName,
                TaskId = taskEntiy.Id,
                StampImg = stampImg,
                FileId = fileId,
                AuditTag = tag,
                TaskCreateDate = taskEntiy.CreationTime,
                IsCancel = isNotCancel ? 0 : 1,
                IsLast = 1
            };
            await _workFlowTaskLogAppService.UpdateLog(taskEntiy.ProcessId, taskEntiy.UnitId);
            await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);
        }

        #endregion

        #region 子流程跳转到父级流程
        /// <summary>
        /// 子流程跳转到父级流程
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task FinishToParent(Guid? taskId)
        {
            if (taskId == null || taskId.IsEmpty())
                throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));

            var taskEntity = await _workFlowTaskSerivce.GetAsync(new EntityDto<Guid>() { Id = taskId.Value });
            if (taskEntity == null)
            {
                throw (new UserFriendlyException("#NotLog#找不到对应流程任务！"));
            }
            var iWFEngine = await Bootstraper("", taskEntity.ProcessId, null, null, null, string.Empty, null);
            if (iWFEngine.Config.Params.IsFinished != 1)
            {
                await AuditNode(iWFEngine, taskEntity, "agree", "子流程同意", "", "", null, "", 0);
            }
            else
            {
                taskEntity.State = 3;
                await _workFlowTaskSerivce.UpdateAsync(taskEntity);
            }
        }

        #endregion

        #region 子流程驳回
        /// <summary>
        /// 子流程跳转到父级流程(驳回)
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<bool> AgainToParent(Guid taskId)
        {
            var taskEntity = await _workFlowTaskSerivce.GetAsync(new EntityDto<Guid>() { Id = taskId });
            if (taskEntity == null)
            {
                return false;
            }
            var iWFEngine = await Bootstraper("", taskEntity.ProcessId, null, null, null, string.Empty, null);
            if (iWFEngine.Config.Params.IsFinished != 1)
            {
                if (taskEntity.IsAuto == 1)
                {
                    await AuditNode(iWFEngine, taskEntity, "disagree", "子流程驳回", "", "", null, "", 0);
                }
                else
                {
                    taskEntity.State = 1;
                    await _workFlowTaskSerivce.UpdateAsync(taskEntity);
                }
                return true;
            }
            else
            {
                taskEntity.State = 3;
                await _workFlowTaskSerivce.UpdateAsync(taskEntity);
                return false;
            }
        }

        #endregion

        #region 普通审批
        /// <summary>
        /// 普通审批
        /// </summary>
        /// <param name="iWFEngine"></param>
        /// <param name="taskEntity"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="des"></param>
        /// <param name="stampImg"></param>
        /// <param name="fileId">审批附件</param>
        /// <param name="IsRejectBackOld">驳回是否原路返回</param>
        /// <param name="nextId">下一个节点</param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task AuditNode(IWorkFlowEngineAppService iWFEngine, WorkFlowTaskDto taskEntity, string code, string name, string des, string stampImg, string fileId, string tag, int? IsRejectBackOld, Guid? nextId = null)
        {
            var testTaskId = taskEntity.Id;
            // 下一部需要执行的任务
            var taskList = await iWFEngine.GetTask(taskEntity.UnitId, taskEntity.IsRejectBackOld, code, nextId, taskEntity.IsReject == 1 ? taskEntity.PrevUnitId : null);
            if (taskList.FindAll(t => t.Type != 10).Count == 0)
            {
                throw (new UserFriendlyException("#NotLog#找不到下一个流转节点！"));
            }
            WorkFlowProcess processEntity = await _processRepository.GetAsync(taskEntity.ProcessId);
            processEntity.IsStart = 1;
            processEntity.IsCancel = 0;
            List<WorkFlowTaskDto> myTaskList;
            try
            {
                // 更新流程状态
                if (taskList.FindIndex(t => t.Type == 100) != -1)
                {
                    processEntity.IsFinished = 1;
                    processEntity.FinishTime = DateTime.Now;
                    processEntity.ProcessMinutes = (int)((DateTime)processEntity.FinishTime - (DateTime)processEntity.CreationTime).TotalMinutes;
                }
                else if (taskList.FindIndex(t => t.Type == 4) != -1)
                {
                    if (iWFEngine.Config.Params.IsChild)
                    {
                        processEntity.IsFinished = 1;
                        processEntity.FinishTime = DateTime.Now;
                        processEntity.ProcessMinutes = (int)((DateTime)processEntity.FinishTime - (DateTime)processEntity.CreationTime).TotalMinutes;
                    }
                    processEntity.IsAgain = 1;
                }
                await _processRepository.UpdateAsync(processEntity);
                // 更新任务状态
                taskEntity.IsAgree = code == "disagree" ? 0 : 1;
                taskEntity.State = 3;
                await _workFlowTaskSerivce.UpdateAsync(taskEntity);

                // 关闭同一个节点，其他人的任务 （放在这，防止锁表）
                await _workFlowTaskSerivce.CloseTask3(taskEntity.ProcessId, taskEntity.Token, taskEntity.Id);
                // 更新上一个流转过来的任务，提示他无法被撤销（放在这，防止锁表）
                await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntity.ProcessId, taskEntity.PrevTaskId);
                if (iWFEngine.Config.Params.IsChild)
                {
                    await _workFlowTaskLogAppService.CloseTaskLogCancel(iWFEngine.Config.Params.ParentProcessId.Value, taskEntity.PrevTaskId);
                }
                myTaskList = await ExecuteWFTaskEntity(taskList, iWFEngine, taskEntity.ProcessId, taskEntity.Token, IsRejectBackOld, taskEntity.Id.ToString());
                foreach (var item in myTaskList)
                {
                    await _workFlowTaskSerivce.Add(item);
                }
                await CreateTaskLog(taskEntity, code, name, des, stampImg, fileId, tag, processEntity.IsFinished == 1);
                // 脚本执行
                var scriptTaskList = myTaskList.FindAll(t => t.Type == 10);
                foreach (var item in scriptTaskList)
                {
                    await ExecuteScript(item, code, des, taskEntity.ProcessId, taskEntity.Id, iWFEngine.CreateUser, iWFEngine, taskEntity);
                }
                // 子流程创建
                var autoChildList = myTaskList.FindAll(t => t.Type == 3 && t.IsAuto == 1);
                WorkFlowProcess parentProcessEntity = null;
                foreach (var childItem in autoChildList)
                {
                    // 自动创建子流程或者再发起子流程
                    var node = iWFEngine.GetNode(childItem.UnitId);
                    var userInfo = await UserManager.GetUserByIdAsync(childItem.UserId.Value);
                    var childIWFEngine = await Bootstraper(string.Empty, childItem.ChildProcessId.Value, node.WfVersionId, childItem.ProcessId, childItem.Id, "", userInfo, null);
                    if (parentProcessEntity == null)
                    {
                        parentProcessEntity = await _processRepository.GetAsync(processEntity.Id);
                    }
                    await CreateFlow(childIWFEngine, null, "", null, parentProcessEntity.Level, parentProcessEntity.SecretLevel, childItem.Token, childItem.Id + "");
                    // 更新任务状态
                    childItem.IsAgree = 1;
                    childItem.State = 8;
                    await _workFlowTaskSerivce.UpdateAsync(childItem);
                    await CreateTaskLog(childItem, "createsub", "子流程提交", "", "", null, "");
                }
                // 如果流程结束且是子流程需要跳转到父级流程
                if (iWFEngine.Config.Params.IsChild)
                {
                    if (processEntity.IsAgain == 1)
                    {
                        if (await AgainToParent(iWFEngine.Config.Params.ParentTaskId.Value))
                        {
                            var againTask = myTaskList.Find(t => t.Type == 4);
                            againTask.State = 3;
                            await _workFlowTaskSerivce.UpdateAsync(againTask);
                        }
                    }
                    else
                    {
                        if (processEntity.IsFinished == 1)
                        {
                            await FinishToParent(iWFEngine.Config.Params.ParentTaskId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        #endregion

        #region 获取子流程
        /// <summary>
        /// 获取子流程
        /// </summary>
        /// <param name="parentTaskId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async Task<WorkFlowProcessDto> GetChildByTaskId(Guid parentTaskId)
        {
            var entity = await _processRepository.GetAll().Where(t => t.ParentTaskId == parentTaskId).FirstOrDefaultAsync();
            return ObjectMapper.Map<WorkFlowProcessDto>(entity);
        }
        #endregion

        #region 删除子流程流转
        /// <summary>
        /// 删除子流程流转
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [HiddenApi]
        public async System.Threading.Tasks.Task RevokeSubAudit(Guid processId, Guid taskId)
        {
            await LockExtensions.Locking(processId.ToString(), async _ =>
            {
                var iWFEngine = await Bootstraper(string.Empty, processId, null, null, null, string.Empty, null);
                try
                {
                    var childTaskList = await _workFlowTaskSerivce.GetTaskList(processId, 3, taskId.ToString());
                    foreach (var item in childTaskList)
                    {
                        if (item.IsAuto == 1)
                        {
                            if (item.State == 1)
                            {
                                throw (new UserFriendlyException("#NotLog#当前任务无法撤销！"));
                            }
                            var childEntity = await this.GetChildByTaskId(item.Id);
                            if (childEntity.IsFinished == 1)
                            {
                                throw new UserFriendlyException("#NotLog#当前任务无法撤销！");
                            }
                            var taskLogEntity = await _workFlowTaskLogAppService.GetLogEntity(new EntityDto<Guid>() { Id = item.Id });
                            if (taskLogEntity == null || taskLogEntity.IsCancel != 1)
                            {
                                throw (new UserFriendlyException("#NotLog#当前任务无法撤销！"));
                            }
                            if (childEntity.IsStart == 1)
                            {
                                await RevokeSubAudit(childEntity.Id, iWFEngine.Config.Params.ParentTaskId ?? Guid.Empty);
                            }
                            else
                            {
                                var res = await RevokeFlow(new EntityDto<Guid>() { Id = childEntity.Id });
                                if (!res)
                                {
                                    throw (new UserFriendlyException("#NotLog#当前任务无法撤销！"));
                                }
                            }
                            await _workFlowTaskLogAppService.DeleteLogByProIdAndTskId(processId, item.Id);
                            await _workFlowTaskLogAppService.DeleteSystemLog(processId, item.UnitId);
                        }
                    }
                    // 获取脚本任务
                    var scriptList = await _workFlowTaskSerivce.GetTaskList(processId, 10, taskId.ToString());
                    foreach (var item in scriptList)
                    {
                        await ExecuteRevokeScript(item.UnitId, item.UnitName, processId, iWFEngine.CreateUser, iWFEngine);
                        // 删除脚本任务执行日志
                        await _workFlowTaskLogAppService.DeleteLogByTaskId(item.Id);
                    }
                    // 删除所有生成的任务
                    await _workFlowTaskLogAppService.DeleteLogByProIdAndTskId(processId, taskId);
                    // 删除所有生成的日志
                    await _workFlowTaskLogAppService.DeleteLogByProIdAndTskId(processId, taskId);
                    var processEntity = new WorkFlowProcess()
                    {
                        Id = processId
                    };
                    processEntity.IsFinished = 0;
                    processEntity.FinishTime = null;
                    processEntity.IsAgain = 0;
                    await _processRepository.UpdateAsync(processEntity);
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            });
        }

        #endregion

        #region 撤销流程
        /// <summary>
        /// 撤销流程（只有在该流程未被处理的情况下）
        /// </summary>
        /// <param name="processId">流程进程主键</param>
        public async Task<bool> RevokeFlow(EntityDto<Guid> input)
        {
            Guid processId = input.Id;
            return await LockExtensions.Locking(processId.ToString(), async _ =>
            {
                var iWFEngine = await Bootstraper(string.Empty, processId, null, null, null, string.Empty, null);
                if (iWFEngine.Config.Params.IsStart != 1 && iWFEngine.Config.Params.IsFinished != 1)
                {
                    // iWFEngine.StartNode.FormVerison; // 绑定的表单版本，通过这个获取自定义表单的模版信息
                    // 判断是否有子流程
                    var childList = await this.GetChildList(processId);
                    if (childList.ToList().FindIndex(t => t.IsStart == 1 || t.IsFinished == 1) > -1)
                    {
                        return false;
                    }
                    // 获取脚本任务
                    var scriptList = await _workFlowTaskSerivce.GetTaskList(processId, 10, "create");
                    foreach (var item in scriptList)
                    {
                        await ExecuteRevokeScript(item.UnitId, item.UnitName, processId, iWFEngine.CreateUser, iWFEngine);
                    }
                    try
                    {
                        foreach (var childItem in childList)
                        {
                            var res = await RevokeFlow(new EntityDto<Guid>() { Id = childItem.Id });
                            await _processRepository.DeleteAsync(childItem.Id); // 删除子流程
                            if (!res)
                            {
                                return false;
                            }
                        }
                        await _workFlowTaskSerivce.DeleteTaskByProcessId(processId);
                        await _workFlowTaskLogAppService.DeleteLogByProcessId(processId);
                        var processentity = await _processRepository.GetAsync(processId);
                        processentity.IsActive = 2;
                        await _processRepository.UpdateAsync(processentity);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    await ExecuteScript(processId, "RevokeFlow", "", iWFEngine.CreateUser,
                    iWFEngine.WFScheme.UndoType,
                    iWFEngine.WFScheme.UndoDbCode,
                    iWFEngine.WFScheme.UndoDbSQL,
                    iWFEngine.WFScheme.UndoUrl,
                    iWFEngine.WFScheme.UndoIOCName);
                    return true;
                }
                return false;
            });
        }

        #endregion

        #region 执行脚本(撤销)

        /// <summary>
        /// 执行脚本(撤销)
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="unitName"></param>
        /// <param name="processId"></param>
        /// <param name="createUser"></param>
        /// <param name="iWFEngine"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task ExecuteRevokeScript(Guid unitId, string unitName, Guid processId, WFUserInfo createUser, IWorkFlowEngineAppService iWFEngine)
        {
            var param = new
            {
                processId,
                userId = createUser.Id,
                userAccount = createUser.Account,
                companyId = createUser.CompanyId,
                departmentId = createUser.DepartmentId,
                code = "revoke",
            };
            try
            {
                var node = iWFEngine.GetNode(unitId);
                switch (node.ExecuteType)
                {
                    case "1":
                        if (!string.IsNullOrEmpty(node.SqlDb) && !string.IsNullOrEmpty(node.SqlStrRevoke) && !string.IsNullOrWhiteSpace(node.SqlStrRevoke))
                        {
                            await _dataBaseService.ExecuteSql(node.SqlStrRevoke, param);
                        }
                        break;
                    case "2":
                        //if (!string.IsNullOrEmpty(node.IocRevoke) && IocManager.Instance.IsRegistered<IWorkFlowMethod>(node.IocRevoke))
                        if (!string.IsNullOrEmpty(node.IocRevoke) && IocManager.Instance.IsRegistered<IWorkFlowMethod>())
                        {
                            IWorkFlowMethod iWorkFlowMethod = IocManager.Instance.Resolve<IWorkFlowMethod>(node.IocRevoke);
                            WfMethodParameter wfMethodParameter = new WfMethodParameter()
                            {
                                ProcessId = processId,
                                UnitName = unitName,
                                Code = "revoke",
                                UserId = createUser.Id,
                                UserAccount = createUser.Account,
                                CompanyId = createUser.CompanyId,
                                DepartmentId = createUser.DepartmentId
                            };
                            await iWorkFlowMethod.Execute(wfMethodParameter);
                        }
                        break;
                    case "3":
                        if (!string.IsNullOrEmpty(node.ApiUrlRevoke) && !string.IsNullOrWhiteSpace(node.ApiUrlRevoke))
                        {
                            if (node.ApiUrlRevoke.IndexOf("?") == -1)
                            {
                                node.ApiUrlRevoke = $"{node.ApiUrlRevoke}?token={RequestPermission.GetRequestToken}";
                            }
                            else
                            {
                                node.ApiUrlRevoke = $"{node.ApiUrlRevoke}&token={RequestPermission.GetRequestToken}";
                            }
                            await HttpMethods.Post(node.ApiUrlRevoke, param.ToJson());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WorkFlowTaskLog wfTaskLogEntity = new WorkFlowTaskLog()
                {
                    Remark = string.Format("脚本执行异常:{0}", ex.Message),
                    TaskType = 99,
                    ProcessId = processId,
                    UnitId = unitId,
                    UnitName = $"{unitName}-撤销"
                };
                await _workFlowTaskLogAppService.AddLog(wfTaskLogEntity);
                var processentity = await _processRepository.GetAsync(processId);
                processentity.IsException = 1;
                await _processRepository.UpdateAsync(processentity);
            }

        }
        /// <summary>
        /// 会签任务
        /// </summary>
        /// <param name="iWFEngine"></param>
        /// <param name="taskEntiy"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="des"></param>
        /// <param name="stampImg"></param>
        /// <param name="fileId"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private async System.Threading.Tasks.Task AuditNodeByCountersign(IWorkFlowEngineAppService iWFEngine, WorkFlowTaskDto taskEntiy, string code, string name, string des, string stampImg, string fileId, string tag)
        {
            try
            {
                taskEntiy.IsAgree = code == "disagree" ? 0 : 1;
                var node = iWFEngine.GetNode(taskEntiy.UnitId);
                var tasklist = await _workFlowTaskSerivce.GetLastTaskList(taskEntiy.ProcessId, taskEntiy.UnitId);
                var list = tasklist.ToList();
                var myIndex = list.FindIndex(t => t.Id == taskEntiy.Id);
                if (myIndex == -1)
                {
                    throw (new UserFriendlyException("#NotLog#会签任务记录异常无法审批！"));
                }
                var isRejectBackOld = 0;
                if (node.IsRejectBackOld)
                {
                    isRejectBackOld = 1;
                }

                if (taskEntiy.IsAgree == 0)
                {
                    if (node.CountersignType == "1")
                    {
                        // 并行
                        if (node.IsCountersignAll)
                        {
                            // 等待
                            if (list.ToList().FindIndex(t => t.Id != taskEntiy.Id && t.State == 1) != -1)
                            {
                                // 表示还有人没有审批
                                // 表示还有任务没有完成
                                try
                                {
                                    var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                                    processEntity.IsStart = 1;
                                    processEntity.IsCancel = 0;
                                    await _processRepository.UpdateAsync(processEntity);
                                    taskEntiy.State = 3;
                                    await _workFlowTaskSerivce.UpdateAsync(taskEntiy);
                                    await CreateTaskLog(taskEntiy, code, name, des, stampImg, fileId, tag);
                                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                            else
                            {
                                var num = await _workFlowTaskSerivce.GetTaskUserMaxNum(taskEntiy.ProcessId, taskEntiy.UnitId);
                                var fnum = list.FindAll(t => t.State == 3 && t.IsAgree == 0).Count + 1;
                                if ((num - fnum) * 100 / num >= node.CountersignAllType)
                                {
                                    await AuditNode(iWFEngine, taskEntiy, "agree", "同意", des, stampImg, fileId, tag, 0);
                                }
                                else
                                {
                                    await AuditNode(iWFEngine, taskEntiy, code, name, des, stampImg, fileId, tag, isRejectBackOld);
                                }
                            }
                        }
                        else
                        {
                            // 不等待，每次都需要计算通过率
                            var num = await _workFlowTaskSerivce.GetTaskUserMaxNum(taskEntiy.ProcessId, taskEntiy.UnitId);
                            var fnum = list.FindAll(t => t.State == 3 && t.IsAgree == 0).Count + 1;
                            if ((num - fnum) * 100 / num >= node.CountersignAllType)// 表示还有通过的希望
                            {
                                try
                                {
                                    var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                                    processEntity.IsStart = 1;
                                    processEntity.IsCancel = 0;
                                    await _processRepository.UpdateAsync(processEntity);
                                    taskEntiy.State = 3;
                                    await _workFlowTaskSerivce.UpdateAsync(taskEntiy);
                                    await CreateTaskLog(taskEntiy, code, name, des, stampImg, fileId, tag);
                                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                            else
                            {
                                await AuditNode(iWFEngine, taskEntiy, code, name, des, stampImg, fileId, tag, isRejectBackOld);
                            }
                        }
                    }
                    else
                    {
                        // 串行
                        await AuditNode(iWFEngine, taskEntiy, code, name, des, stampImg, fileId, tag, isRejectBackOld);
                    }
                }
                else
                {
                    if (node.CountersignType == "1")
                    {
                        // 并行
                        // 如果不等待，不同意直接跳转
                        if (node.IsCountersignAll)
                        {
                            if (list.ToList().FindIndex(t => t.Id != taskEntiy.Id && t.State == 1) != -1) // 表示还有人没有处理完
                            {
                                // 表示还有任务没有完成
                                try
                                {
                                    var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                                    processEntity.IsStart = 1;
                                    processEntity.IsCancel = 0;
                                    await _processRepository.UpdateAsync(processEntity);
                                    taskEntiy.State = 3;
                                    await _workFlowTaskSerivce.UpdateAsync(taskEntiy);
                                    await CreateTaskLog(taskEntiy, code, name, des, stampImg, fileId, tag);
                                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                            else
                            {
                                var num = await _workFlowTaskSerivce.GetTaskUserMaxNum(taskEntiy.ProcessId, taskEntiy.UnitId);
                                var fnum = list.FindAll(t => t.State == 3 && t.IsAgree == 0).Count;
                                if ((num - fnum) * 100 / num >= node.CountersignAllType)
                                {
                                    await AuditNode(iWFEngine, taskEntiy, "agree", "同意", des, stampImg, fileId, tag, 0);
                                }
                                else
                                {
                                    await AuditNode(iWFEngine, taskEntiy, "disagree", "不同意", des, stampImg, fileId, tag, isRejectBackOld);
                                }
                            }
                        }
                        else
                        {
                            var num = await _workFlowTaskSerivce.GetTaskUserMaxNum(taskEntiy.ProcessId, taskEntiy.UnitId);
                            var snum = list.FindAll(t => t.State == 3 && t.IsAgree == 1).Count + 1;
                            if (snum * 100 / num >= node.CountersignAllType)
                            {
                                await AuditNode(iWFEngine, taskEntiy, "agree", "同意", des, stampImg, fileId, tag, 0);
                            }
                            else
                            {
                                try
                                {
                                    var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                                    processEntity.IsStart = 1;
                                    processEntity.IsCancel = 0;
                                    await _processRepository.UpdateAsync(processEntity);
                                    taskEntiy.State = 3;
                                    await _workFlowTaskSerivce.UpdateAsync(taskEntiy);
                                    await CreateTaskLog(taskEntiy, code, name, des, stampImg, fileId, tag);
                                    await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 串行
                        if (myIndex == list.Count - 1) // 表示最后一个人审批完成，然后往下执行
                        {
                            await AuditNode(iWFEngine, taskEntiy, code, name, des, stampImg, fileId, tag, 0);
                        }
                        else
                        {
                            // 表示还有任务没有完成
                            try
                            {
                                var processEntity = await _processRepository.GetAsync(taskEntiy.ProcessId);
                                processEntity.IsStart = 1;
                                processEntity.IsCancel = 0;
                                await _processRepository.UpdateAsync(processEntity);
                                taskEntiy.State = 3;
                                await _workFlowTaskSerivce.UpdateAsync(taskEntiy);
                                await CreateTaskLog(taskEntiy, code, name, des, stampImg, fileId, tag);
                                if (node.CountersignType == "2")
                                {
                                    // 串行，更新上一个任务的撤销操作，开启下一个任务
                                    if (myIndex == 0)
                                    {
                                        await _workFlowTaskLogAppService.CloseTaskLogCancel(taskEntiy.ProcessId, taskEntiy.PrevTaskId);
                                    }
                                    else
                                    {
                                        await _workFlowTaskLogAppService.CloseTaskLogCancelByTaskId(list[myIndex - 1].Id);
                                    }
                                    taskEntiy.State = 1;
                                    await _workFlowTaskSerivce.UpdateAsync(taskEntiy);
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ADTOSharpException(L(ex.Message));
            }

        }
        #endregion

        #region 执行流程服务

        #endregion



    }
}

