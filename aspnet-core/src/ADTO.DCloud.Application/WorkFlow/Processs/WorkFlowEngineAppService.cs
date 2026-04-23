using ADTO.DCloud.Authorization;
using ADTO.DCloud.DataBase;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.WorkFlow.Processs.Config;
using ADTO.DCloud.WorkFlow.Processs.Dto;
using ADTO.DCloud.WorkFlow.Tasks;
using ADTOSharp;
using ADTOSharp.Authorization;
using ADTOSharp.UI;
using Castle.Components.DictionaryAdapter;
using Dapper;
using Newtonsoft.Json.Linq;
using NuGet.Packaging;
using NuGet.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ADTO.DCloud.WorkFlow.Processs
{
    /// <summary>
    /// 工作流引擎
    /// </summary>
    //[ADTOSharpAuthorize(PermissionNames.Pages_Workflow_Process)]
    [ADTOSharpAuthorize]
    public class WorkFlowEngineAppService : IWorkFlowEngineAppService
    {

        private readonly DataBaseService _dataBaseService;
        /// <summary>
        /// 流程配置信息
        /// </summary>
        private WFEngineConfig _config { get; set; }
        /// <summary>
        /// 流程模板
        /// </summary>
        private WFScheme _WorkFlowScheme { get; set; }
        /// <summary>
        /// 流程单元对应字典
        /// </summary>
        private Dictionary<Guid, WorkFlowUnit> _dicUnit { get; set; }
        /// <summary>
        /// 开始节点
        /// </summary>
        private WorkFlowUnit _startNode { get; set; }
        /// <summary>
        /// 结束节点
        /// </summary>
        private WorkFlowUnit _endNode { get; set; }
        /// <summary>
        /// 线条集合
        /// </summary>
        private List<WorkFlowUnit> _lines { get; set; }


        private readonly IDapperSqlExecutor _sqlExecutor;
        private readonly IGuidGenerator _guidGenerator;
        #region 构造函数
        public WorkFlowEngineAppService(DataBaseService dataBaseService,
        IDapperSqlExecutor sqlExecutor,
        IGuidGenerator guidGenerator)
        {
            _dataBaseService = dataBaseService;
            _sqlExecutor = sqlExecutor;
            _guidGenerator = guidGenerator;
        }
        #endregion

        #region 初始化配置
        /// <summary>
        /// 初始化工作流引擎（代替原来的构造函数逻辑）
        /// </summary>
        public void Initialize(WFEngineConfig config)
        {
            _config = config;
            _WorkFlowScheme = _config.Params.Scheme.ToObject<WFScheme>();
            _dicUnit = new Dictionary<Guid, WorkFlowUnit>();
            _lines = new List<WorkFlowUnit>();

            // 原有的初始化逻辑...
            foreach (var unit in _WorkFlowScheme.WfData)
            {
                if (!_dicUnit.ContainsKey(unit.Id))
                {
                    _dicUnit.Add(unit.Id, unit);
                }
                else
                {
                    continue;
                }
                if (unit.Type == "startEvent")
                {
                    if (string.IsNullOrEmpty(unit.Name))
                    {
                        unit.Name = "开始节点";
                    }
                    _startNode = unit;
                }
                else if (unit.Type == "endEvent")
                {
                    _endNode = unit;
                }
                else if (unit.Type == "myline")
                {
                    _lines.Add(unit);
                }
            }
        }
        #endregion
        /// <summary>
        /// 流程运行参数
        /// </summary>
        public WFEngineConfig Config => _config;
        /// <summary>
        /// 开始节点
        /// </summary>
        public WorkFlowUnit StartNode => _startNode;

        /// <summary>
        /// 结束节点
        /// </summary>
        public WorkFlowUnit EndNode => _endNode;

        /// <summary>
        /// 流程发起用户
        /// </summary>
        public WFUserInfo CreateUser => _config.Params.CreateUser;
        /// <summary>
        /// 流程发起用户
        /// </summary>
        public WFUserInfo CurrentUser => _config.Params.CurrentUser;
        /// <summary>
        /// 流程模板
        /// </summary>
        public WFScheme WFScheme => _WorkFlowScheme;

        /// <summary>
        /// 流程配置信息
        /// </summary>
        public WFScheme WorkFlowScheme => _WorkFlowScheme;


        /// <summary>
        /// 获取流程单元信息taskid-> nodeid
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>流程单元信息</returns>
        public WorkFlowUnit GetNode(Guid id)
        {
            if (_dicUnit.ContainsKey(id))
            {
                return _dicUnit[id];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取下一节点集合
        /// </summary>
        /// <param name="startId">开始节点</param>
        /// <param name="code">执行动作编码</param>
        /// <returns></returns>
        public List<WorkFlowUnit> GetNextUnits(Guid startId, string code = "")
        {
            List<WorkFlowUnit> nextUnits = new List<WorkFlowUnit>();
            // 找到与当前节点相连的线条
            foreach (var line in _lines)
            {
                if (line.From == startId)
                {
                    // 获取连接到开始节点的线条
                    bool isOk = false;
                    if (string.IsNullOrEmpty(line.LineConditions))
                    {
                        isOk = true;
                    }
                    else if (!string.IsNullOrEmpty(code))
                    {
                        var codeList = line.LineConditions.Split(',');
                        foreach (string _code in codeList)
                        {
                            if (_code == code)
                            {
                                isOk = true;
                                break;
                            }
                        }
                    }
                    else
                    { //没有执行码就是全部执行
                        isOk = true;
                    }
                    // 获取到流入节点
                    if (isOk)
                    {
                        if (_dicUnit.ContainsKey(line.To) && nextUnits.Find(t => t.Id == line.To) == null)
                        {
                            nextUnits.Add(_dicUnit[line.To]);
                        }
                    }
                }
            }
            return nextUnits;
        }
        /// <summary>
        /// 获取下一节点集合
        /// </summary>
        /// <param name="startId">开始节点</param>
        /// <param name="codeList">条件编码</param>
        /// <returns></returns>
        public List<WorkFlowUnit> GetNextUnitLists(Guid startId, List<string> codeList)
        {
            List<WorkFlowUnit> nextUnits = new List<WorkFlowUnit>();
            // 找到与当前节点相连的线条
            foreach (var line in _lines)
            {
                if (line.From == startId)
                {
                    // 获取连接到开始节点的线条
                    bool isOk = false;
                    if (codeList.Count > 0 && !string.IsNullOrEmpty(line.LineConditions))
                    {
                        var codeList2 = line.LineConditions.Split(',');
                        foreach (string _code in codeList2)
                        {
                            if (codeList.FindIndex(t => t == _code) != -1)
                            {
                                isOk = true;
                                break;
                            }
                        }
                    }
                    // 获取到流入节点
                    if (isOk)
                    {
                        if (_dicUnit.ContainsKey(line.To) && nextUnits.Find(t => t.Id == line.To) == null)
                        {
                            nextUnits.Add(_dicUnit[line.To]);
                        }
                    }
                }
            }
            return nextUnits;
        }

        /// <summary>
        /// 获取下一节点集合（流转条件为空的情况）
        /// </summary>
        /// <param name="startId">开始节点</param>
        /// <returns></returns>
        public List<WorkFlowUnit> GetNextUnitsNoSet(Guid startId)
        {
            List<WorkFlowUnit> nextUnits = new List<WorkFlowUnit>();

            // 找到与当前节点相连的线条
            foreach (var line in _lines)
            {
                if (line.From == startId)
                {
                    // 获取到流入节点
                    if (string.IsNullOrEmpty(line.LineConditions))
                    {
                        if (_dicUnit.ContainsKey(line.To) && nextUnits.Find(t => t.Id == line.To) == null)
                        {
                            nextUnits.Add(_dicUnit[line.To]);
                        }
                    }
                }
            }
            return nextUnits;
        }

        /// <summary>
        /// 获取上一单元ID列表
        /// </summary>
        /// <param name="myUnitId">当前节点Id</param>
        /// <returns></returns>
        public List<Guid> GetPreUnitIds(Guid myUnitId)
        {
            List<Guid> list = new List<Guid>();
            // 找到与当前节点相连的线条
            foreach (var line in _lines)
            {
                if (line.To == myUnitId && list.Find(t => t == line.From) == Guid.Empty)
                {
                    list.Add(line.From);
                }
            }
            return list;
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="startId">开始节点</param>
        /// <param name="isRejectBackOld">是否原路返回 1 是 其它否</param>
        /// <param name="code">执行动作编码</param>
        /// <param name="toUnitId">下一个指定节点</param>
        /// <param name="rejectNode">驳回节点</param>
        /// <param name="fromId">来自的节点</param>
        /// <param name="IsGetNextAuditors">用来获取下一审批人</param>
        /// <returns></returns>
        public async Task<List<WFTask>> GetTask(Guid startId, int? isRejectBackOld, string code = "", Guid? toUnitId = null, Guid? rejectNode = null, Guid? fromId = null, bool IsGetNextAuditors = false)
        {
            bool isIsReject = code == "disagree";
            var nextUnits = new List<WorkFlowUnit>();
            var myUnit = GetNode(startId);

            if (!rejectNode.IsEmpty() && code != "disagree" && isRejectBackOld == 1)// code == "" 重新提交
            {
                // 驳回按原路返回
                nextUnits.Add(GetNode(rejectNode.Value));
            }
            else
            {
                nextUnits = GetNextUnits(startId, code);
                if (!toUnitId.IsEmpty())
                {
                    nextUnits = nextUnits.FindAll(t => t.Type == "scriptTask");// 只保留脚本节点
                    nextUnits.Add(GetNode(toUnitId.Value));
                }

                // 如果无法获取下一个审批任务且是驳回操作,就返回到之前流转过来的任务(除去通过驳回流转过来的)（需要过滤掉脚本节点）
                if (nextUnits.FindAll(t => t.Type != "scriptTask").Count == 0 && isIsReject)
                {
                    // 获取当前节点的
                    var prevUnitId = await Config.GetPrevUnitId(Config.Params.ProcessId, startId, fromId ?? Guid.Empty);
                    if (!prevUnitId.IsEmpty())
                    {
                        nextUnits.Add(GetNode(prevUnitId));
                    }
                }
            }
            var list = await GetTask1(nextUnits, isIsReject, startId, isRejectBackOld, IsGetNextAuditors);
            foreach (var item in list)
            {
                item.PrevUnitId = startId;
            }
            return list;
        }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="nextUnits">下一个节点集合</param>
        /// <param name="isReject">是否驳回</param>
        /// <param name="startId">开始节点</param>
        /// <param name="isRejectBackOld">是否原路返回 1 是 其它否</param>
        /// <param name="IsGetNextAuditors">用来获取下一审批人</param>
        /// <returns></returns>
        public async Task<List<WFTask>> GetTask1(List<WorkFlowUnit> nextUnits, bool isReject, Guid startId, int? isRejectBackOld, bool IsGetNextAuditors)
        {
            WorkFlowUnit startUnit = _dicUnit[startId];

            if (startUnit.Type == "startEvent" && string.IsNullOrEmpty(startUnit.Name))
            {
                startUnit.Name = "开始节点";
            }
            List<WFTask> taskList = new List<WFTask>();

            foreach (var unit in nextUnits)
            {
                switch (unit.Type)
                {
                    case "startEvent":// 开始节点
                        taskList.Add(GetStartTask(unit, isReject, startId, startUnit.Name));
                        break;
                    case "gatewayAnd":
                        // 并行网关会等待所有分支汇入才往下执行，所有出口分支都会被执行
                        // 判断是否所有分支都被执行了
                        // 1.或去此节点所有上节点
                        // 2.生成一个等待任务
                        // 3.获取所有该节点当前等待任务
                        // 4.判断是否完成获取下一节点
                        var preIdList = GetPreUnitIds(unit.Id);
                        if (preIdList.Count <= 1)
                        {
                            taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, "", null, null, null, IsGetNextAuditors));
                        }
                        else
                        {
                            var awaitTaskList = await Config.GetAwaitTaskList(Config.Params.ProcessId, unit.Id);
                            var awaitTaskList2 = awaitTaskList.FindAll(t => t.PrevUnitId != startId); // 排除当前等待任务的所有等待任务
                            if (awaitTaskList2.Count + 1 >= preIdList.Count)
                            {
                                // 如果所有支路都被执行了就生成一个取消所有等待任务的命令
                                taskList.Add(GetDeleteAwaitTask(unit, isReject, startId, startUnit.Name));
                                taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, "", null, null, null, IsGetNextAuditors));
                            }
                            else
                            {
                                taskList.Add(GetAwaitTask(unit, isReject, startId, startUnit.Name));
                            }
                        }
                        break;
                    case "gatewayXor":
                        //  排他网关不会等待所有分支汇入才往下执行，只要有分支汇入就会往下执行，出口分支只会执行一条（条件为true，如果多条出口分支条件为true也执行一条）
                        // 1.获取当前节点的true条件，如果成立条件为0，就执行人默认没有条件的线条
                        // 2.获取下一个节点
                        var conditionCodes = await GetConditionIds(unit.Conditions);
                        var nextUnits2 = GetNextUnitLists(unit.Id, conditionCodes);
                        if (nextUnits2.Count == 0)
                        {
                            nextUnits2 = GetNextUnitsNoSet(unit.Id);
                        }
                        if (nextUnits2.Count == 0)
                        {
                            throw new Exception(string.Format("无法获取下一节点【{0}】", unit.Id));
                        }
                        else
                        {
                            var nextUnits3 = new List<WorkFlowUnit>();
                            nextUnits3.Add(nextUnits2[0]);
                            taskList.AddRange(await GetTask1(nextUnits3, isReject, unit.Id, isRejectBackOld, IsGetNextAuditors));
                        }
                        break;
                    case "gatewayInclusive":

                        // 包容网关会等待所有分支汇入才往下执行，出口分支能执行多条（条件为true）
                        bool isNext = false;
                        var preIdList2 = GetPreUnitIds(unit.Id);
                        if (preIdList2.Count <= 1)
                        {
                            isNext = true;
                        }
                        else
                        {
                            var awaitTaskList = await Config.GetAwaitTaskList(Config.Params.ProcessId, unit.Id);
                            var awaitTaskList2 = awaitTaskList.FindAll(t => t.PrevUnitId != startId); // 排除当前等待任务的所有等待任务
                            if (awaitTaskList2.Count + 1 >= preIdList2.Count)
                            {
                                // 如果所有支路都被执行了就生成一个取消所有等待任务的命令
                                taskList.Add(GetDeleteAwaitTask(unit, isReject, startId, startUnit.Name));
                                isNext = true;
                            }
                            else
                            {
                                taskList.Add(GetAwaitTask(unit, isReject, startId, startUnit.Name));
                            }
                        }
                        if (isNext)
                        {
                            var conditionCodes2 = await GetConditionIds(unit.Conditions);
                            var nextUnits3 = GetNextUnitLists(unit.Id, conditionCodes2);
                            nextUnits3.AddRange(GetNextUnitsNoSet(unit.Id));
                            taskList.AddRange(await GetTask1(nextUnits3, isReject, unit.Id, isRejectBackOld, IsGetNextAuditors));
                        }
                        break;
                    case "userTask":
                        // 审批节点
                        // 1.获取节点上一次的请求人
                        var preAuditUsers = await Config.GetPrevTaskUserList(Config.Params.ProcessId, unit.Id);
                        if (preAuditUsers.FindIndex(t => t.State == 1) != -1)
                        {
                            // 如果此节点被激活不需要审批
                            taskList.Add(new WFTask { Type = 99 });
                            break;
                        }
                        if (isRejectBackOld == 1 && preAuditUsers.FindIndex(t => t.IsSign) > -1)
                        {
                            preAuditUsers = preAuditUsers.FindAll(t => t.IsSign && (t.State == 2 || t.State == 4 || !t.IsAgree));
                            var signTaskList = GetUserTaskList(unit, preAuditUsers, isReject, startId, startUnit.Name,
                                _guidGenerator.Create().ToString());
                            foreach (var signTaskItem in signTaskList)
                            {
                                signTaskItem.Type = 6;
                                if (signTaskItem.User.State == 2)
                                {
                                    signTaskItem.User.IsAwait = true;
                                }
                                taskList.Add(signTaskItem);
                            }
                            return taskList;
                        }
                        else
                        {
                            preAuditUsers = preAuditUsers.FindAll(t => !t.IsSign);
                        }

                        var auditUsers = new List<WFUserInfo>();
                        if (preAuditUsers.Count == 0 || (unit.IsUpdateAuditor && isRejectBackOld != 1))
                        {
                            // 表示此节点未被处理过
                            // 获取审批人
                            var auditUserList = new List<WorkFlowAuditor>();
                            if (Config.Params.NextUsers != null && Config.Params.NextUsers.ContainsKey(unit.Id))
                            {
                                var strAuditUser = Config.Params.NextUsers[unit.Id];
                                var strAuditUserList = strAuditUser.Split(",");
                                foreach (var id in strAuditUserList)
                                {
                                    auditUserList.Add(new WorkFlowAuditor()
                                    {
                                        Type = "3",
                                        Id = Guid.Parse(id)
                                    });
                                }
                            }
                            else
                            {
                                if (unit.AuditUsers != null)
                                {
                                    auditUserList.AddRange(unit.AuditUsers);
                                }
                                if (unit.AuditorUsersCard != null)
                                {
                                    auditUserList.AddRange(unit.AuditorUsersCard);
                                }
                            }
                            auditUsers = await Config.GetUserList(auditUserList, CreateUser, Config.Params.ProcessId, unit, startUnit, _startNode);
                            if (preAuditUsers.Count == 0)
                            {
                                // 添加传阅任务，节点第一次执行的时候触发
                                var lookUserList = new List<WorkFlowAuditor>();
                                if (unit.LookUsers != null)
                                {
                                    lookUserList.AddRange(unit.LookUsers);
                                }
                                if (unit.LookUsersCard != null)
                                {
                                    lookUserList.AddRange(unit.LookUsersCard);
                                }
                                var lookUsers = await Config.GetUserList(lookUserList, CreateUser, Config.Params.ProcessId, unit, startUnit, _startNode);
                                taskList.AddRange(GetLookUserTaskList(unit, lookUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                            }
                        }
                        else
                        {
                            // 添加一条任务用于更新之前任务状态(将之前的任务设置成不是最近的一次任务)
                            taskList.Add(GetUpdateTask(unit));
                        }
                        // 表示找不到审批人
                        if (preAuditUsers.Count == 0 && auditUsers.Count == 0)
                        {
                            if (IsGetNextAuditors && unit.NoAuditor != "1")
                            {
                                taskList.AddRange(GetUserTaskListNoUsers(unit, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                            }
                            else
                            {
                                switch (unit.NoAuditor)
                                {
                                    case "1":
                                        taskList.AddRange(GetUserTaskList(unit, await Config.GetSystemUserList(), isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                                        break;
                                    case "2":
                                        // 流转到下一个节点
                                        taskList.Add(GetSkipTask(unit, isReject, startId, startUnit.Name));
                                        taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, isReject ? "disagree" : "", null, null, startId, IsGetNextAuditors));
                                        break;
                                    case "3":
                                        throw new UserFriendlyException(string.Format("【{0}{1}】找不到审批人，无法提交", unit.Name, unit.Id));
                                    case "4":
                                        // 退回到发起节点
                                        taskList.Add(GetSkipStartTask(unit, isReject, startId, startUnit.Name));
                                        taskList.AddRange(GetStartTask2(startId, isReject));
                                        break;
                                }
                            }
                        }
                        // 1.需要先判断是否是会签
                        if (unit.IsCountersign)
                        {
                            // 之前有人处理过
                            if (preAuditUsers.Count > 0)
                            {
                                // 驳回处理，重新开启审批
                                if (isReject)
                                {
                                    taskList.AddRange(GetCountersignTask(unit, preAuditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString(), unit.CountersignType));
                                }
                                else
                                // 1.判断之前的会签策略是否通过
                                if (await Config.IsCountersignAgree(Config.Params.ProcessId, unit.Id))
                                {
                                    // 判断自动同意规则，会签状态下只有3有效
                                    if (unit.AutoAgree.IndexOf("3") != -1)
                                    {
                                        taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, "", null, null, null, IsGetNextAuditors));// 自动审批，流转到下一节点
                                    }
                                    else
                                    {
                                        taskList.AddRange(GetCountersignTask(unit, preAuditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString(), unit.CountersignType));
                                    }
                                }
                                else
                                {
                                    taskList.AddRange(GetCountersignTask(unit, preAuditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString(), unit.CountersignType, unit.CountersignAgian));
                                }
                            }
                            // 第一次处理
                            else
                            {
                                taskList.AddRange(GetCountersignTask(unit, auditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString(), unit.CountersignType));
                            }
                        }
                        else
                        {
                            // 之前有人处理过
                            if (preAuditUsers.Count > 0)
                            {
                                if (IsAtuoAgree(preAuditUsers, unit.AutoAgree.Split(','), isReject))
                                {
                                    taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, "agree", null, null, null, IsGetNextAuditors));
                                }
                                else
                                {
                                    if (unit.IsUpdateAuditor && isRejectBackOld != 1)
                                    {
                                        taskList.AddRange(GetUserTaskList(unit, auditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                                    }
                                    // 判断是否有完成任务的人
                                    else if (preAuditUsers.FindIndex(t => t.State == 3) != -1)
                                    {
                                        taskList.AddRange(GetUserTaskList(unit, preAuditUsers.FindAll(t => t.State == 3), isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                                    }
                                    else
                                    {
                                        taskList.AddRange(GetUserTaskList(unit, preAuditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                                    }
                                }
                            }
                            else
                            {
                                if (IsAtuoAgree(auditUsers, unit.AutoAgree.Split(','), isReject))
                                {
                                    taskList.Add(GetAutoSkipTask(unit, isReject, startId, startUnit.Name));// 作为日志记录
                                    taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, "agree", null, null, null, IsGetNextAuditors));
                                }
                                else
                                {
                                    taskList.AddRange(GetUserTaskList(unit, auditUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                                }
                            }
                        }
                        break;
                    case "scriptTask":// 脚本节点
                        taskList.Add(GetScriptTaskList(unit, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                        taskList.AddRange(await GetTask(unit.Id, isRejectBackOld, "", null, null, null, IsGetNextAuditors));
                        break;
                    case "subprocess":// 子节点
                        var preUsers = await Config.GetPrevTaskUser(Config.Params.ProcessId, unit.Id);
                        if (preUsers != null && (preUsers.State == 1 || preUsers.State == 8))
                        {
                            // 如果此节点被激活不需要审批
                            taskList.Add(new WFTask { Type = 99 });
                            break;
                        }
                        var subUsers = new List<WFUserInfo>();
                        if (unit.IsAuto)
                        {
                            // 自定发起子流程用当前操作人员作为子流程创建者
                            var auditUserList = new List<WorkFlowAuditor>();
                            auditUserList.Add(new WorkFlowAuditor()
                            {
                                Type = "3",
                                Id = Config.Params.CurrentUser.Id
                            });
                            subUsers = await Config.GetUserList(auditUserList, CreateUser, Config.Params.ProcessId, unit, startUnit, _startNode);

                            if (preUsers == null)
                            {
                                // 添加脚本
                                var subTaskList = GetSubTask(unit, subUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString());
                                taskList.AddRange(subTaskList);
                                var subScripttask = GetSubScriptTask(unit, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString());
                                if (subScripttask != null)
                                {
                                    subScripttask.ChildProcessId = subTaskList[0].ChildProcessId;
                                    taskList.Add(subScripttask);
                                }
                            }
                            else
                            {
                                subUsers[0].ChildProcessId = preUsers.ChildProcessId;
                                taskList.AddRange(GetSubTask(unit, subUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                                // 添加一条任务用于更新之前任务状态(将之前的任务设置成不是最近的一次任务)
                                taskList.Add(GetUpdateTask(unit));
                            }
                        }
                        else
                        {
                            if (preUsers == null)
                            {
                                var auditUserList = new List<WorkFlowAuditor>();
                                if (Config.Params.NextUsers != null && Config.Params.NextUsers.ContainsKey(unit.Id))
                                {
                                    var strAuditUser = Config.Params.NextUsers[unit.Id];
                                    var strAuditUserList = strAuditUser.Split(",");
                                    foreach (var id in strAuditUserList)
                                    {
                                        auditUserList.Add(new WorkFlowAuditor()
                                        {
                                            Type = "3",
                                            Id = Guid.Parse(id)
                                        });
                                    }
                                }
                                else
                                {
                                    if (unit.AuditUsers != null)
                                    {
                                        auditUserList.AddRange(unit.AuditUsers);
                                    }
                                    if (unit.AuditorUsersCard != null)
                                    {
                                        auditUserList.AddRange(unit.AuditorUsersCard);
                                    }
                                }
                                subUsers = await Config.GetUserList(auditUserList, CreateUser, Config.Params.ProcessId, unit, startUnit, _startNode);

                                var subTaskList = new List<WFTask>();

                                if (subUsers.Count == 0)
                                {
                                    subTaskList = GetSubTask(unit, await Config.GetSystemUserList(), isReject, startId, startUnit.Name, _guidGenerator.Create().ToString());
                                }
                                else
                                {
                                    subTaskList = GetSubTask(unit, subUsers, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString());
                                }
                                taskList.AddRange(subTaskList);
                                var subScripttask = GetSubScriptTask(unit, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString());
                                if (subScripttask != null)
                                {
                                    subScripttask.ChildProcessId = subTaskList[0].ChildProcessId;
                                    taskList.Add(subScripttask);
                                }
                            }
                            else
                            {
                                // 添加一条任务用于更新之前任务状态(将之前的任务设置成不是最近的一次任务)
                                taskList.Add(GetUpdateTask(unit));
                                var preUserList = new List<WFUserInfo>();
                                preUserList.Add(preUsers);
                                taskList.AddRange(GetSubTask(unit, preUserList, isReject, startId, startUnit.Name, _guidGenerator.Create().ToString()));
                            }
                        }

                        break;
                    case "endEvent":// 结束节点
                        var messageUserList = new List<WorkFlowAuditor>();
                        if (unit.AuditUsers != null)
                        {
                            messageUserList.AddRange(unit.AuditUsers);
                        }
                        if (unit.AuditorUsersCard != null)
                        {
                            messageUserList.AddRange(unit.AuditorUsersCard);
                        }
                        if (messageUserList.Count > 0)
                        {
                            // 节点消息
                            auditUsers = await Config.GetUserList(messageUserList, CreateUser, Config.Params.ProcessId, unit, startUnit, _startNode);
                            taskList.AddRange(GetEndMsgTask(unit, auditUsers, startId, startUnit.Name));
                        }
                        taskList.Add(GetEndTask(unit, startId, startUnit.Name));

                        break;
                }
            }

            return taskList;
        }
        /// <summary>
        /// 获取一个找不到审批人直接跳过任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IsReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private WFTask GetAutoSkipTask(WorkFlowUnit unit, bool IsReject, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = "自动审批规则跳过",
                Type = 24,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = IsReject
            };
            return task;
        }

        /// <summary>
        /// 获取一个更新任务
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private WFTask GetUpdateTask(WorkFlowUnit unit)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Type = 26,
            };
            return task;
        }
        /// <summary>
        /// 获取一个取消等待任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IsReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private WFTask GetDeleteAwaitTask(WorkFlowUnit unit, bool IsReject, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = "取消等待任务",
                Type = 22,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = IsReject
            };
            return task;
        }
        /// <summary>
        /// 获取一个等待任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IsReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private WFTask GetAwaitTask(WorkFlowUnit unit, bool IsReject, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = "等待其它支路完成",
                Type = 21,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = IsReject
            };
            return task;
        }
        /// <summary>
        /// 获取审批任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="userList"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<WFTask> GetUserTaskList(WorkFlowUnit unit, List<WFUserInfo> userList, bool isReject, Guid preUnitId, string preUnitName, string token)
        {
            List<WFTask> list = new List<WFTask>();
            foreach (var user in userList)
            {
                WFTask task = new WFTask()
                {
                    UnitId = unit.Id,
                    NodeCode = unit.NodeCode,
                    Name = unit.Name,
                    Type = 1,
                    PrevUnitId = preUnitId,
                    PrevUnitName = preUnitName,
                    IsReject = isReject,
                    Token = token,
                    MessageType = unit.MessageType,

                    // 超时设置
                    IsOvertimeMessage = unit.IsOvertimeMessage,
                    OvertimeMessageStart = unit.OvertimeMessageStart,
                    OvertimeMessageInterval = unit.OvertimeMessageInterval,
                    IsOvertimeGo = unit.IsOvertimeGo,
                    OvertimeGo = unit.OvertimeGo,
                    OvertimeMessageType = unit.OvertimeMessageType,

                    IsBatchAudit = unit.IsBatchAudit,
                    Step = unit.Step,
                    AuditTime = unit.AuditTime,
                    // 处理人
                    User = user
                };
                if (string.IsNullOrEmpty(task.Name))
                {
                    task.Name = "审批处理";
                }
                list.Add(task);
            }
            return list;
        }

        /// <summary>
        /// 获取传阅任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="userList"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<WFTask> GetLookUserTaskList(WorkFlowUnit unit, List<WFUserInfo> userList, bool isReject, Guid preUnitId, string preUnitName, string token)
        {
            List<WFTask> list = new List<WFTask>();
            foreach (var user in userList)
            {
                WFTask task = new WFTask()
                {
                    UnitId = unit.Id,
                    NodeCode = unit.NodeCode,
                    Name = unit.Name,
                    Type = 2,
                    PrevUnitId = preUnitId,
                    PrevUnitName = preUnitName,
                    IsReject = isReject,
                    Token = token,
                    MessageType = unit.MessageType,

                    // 处理人
                    User = user
                };
                if (string.IsNullOrEmpty(task.Name))
                {
                    task.Name = "查看";
                }

                list.Add(task);
            }
            return list;
        }

        /// <summary>
        /// 获取审批任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="userList"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<WFTask> GetUserTaskListNoUsers(WorkFlowUnit unit, bool isReject, Guid preUnitId, string preUnitName, string token)
        {
            List<WFTask> list = new List<WFTask>();
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = unit.Name,
                Type = 1,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = isReject,
                Token = token,
                MessageType = unit.MessageType,

                // 超时设置
                IsOvertimeMessage = unit.IsOvertimeMessage,
                OvertimeMessageStart = unit.OvertimeMessageStart,
                OvertimeMessageInterval = unit.OvertimeMessageInterval,
                IsOvertimeGo = unit.IsOvertimeGo,
                OvertimeGo = unit.OvertimeGo,
                OvertimeMessageType = unit.OvertimeMessageType,

                IsBatchAudit = unit.IsBatchAudit,
                Step = unit.Step,
                AuditTime = unit.AuditTime,
            };
            if (string.IsNullOrEmpty(task.Name))
            {
                task.Name = "审批处理";
            }

            list.Add(task);


            return list;
        }
        /// <summary>
        /// 获取一个找不到审批人直接跳过任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IsReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private WFTask GetSkipTask(WorkFlowUnit unit, bool IsReject, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = "当前任务找不到审批人直接跳过",
                Type = 23,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = IsReject
            };

            return task;
        }

        private WFTask GetSkipStartTask(WorkFlowUnit unit, bool IsReject, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = "当前任务找不到审批人跳转到开始节点",
                Type = 27,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = IsReject
            };

            return task;
        }
        /// <summary>
        /// 获取开始节点任务
        /// </summary>
        /// <param name="startId"></param>
        /// <param name="isReject"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<WFTask> GetStartTask2(Guid startId, bool isReject)
        {
            WorkFlowUnit startUnit = _dicUnit[startId];
            if (startUnit.Type == "startEvent")
            {
                throw new Exception("开始节点无法跳转到自己，请查看模版设置！");
            }
            List<WFTask> res = new List<WFTask>();
            res.Add(GetStartTask(_startNode, isReject, startId, startUnit.Name));
            return res;
        }
        /// <summary>
        /// 获取会签审批任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="userList"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <param name="countersignType">审批方式 1.并行 2.串行</param>
        /// <param name="countersignAgian">再次审批 1.已同意不需要审批 2.已同意需要审批</param>
        /// <returns></returns>
        private List<WFTask> GetCountersignTask(WorkFlowUnit unit, List<WFUserInfo> userList, bool isReject, Guid preUnitId, string preUnitName, string token, string countersignType, string countersignAgian = "2")
        {
            List<WFTask> list = new List<WFTask>();
            int num = 1;
            foreach (var user in userList)
            {
                if (countersignAgian == "1" && user.IsAgree)
                {
                    continue;
                }

                user.IsAwait = countersignType == "1" ? false : true;
                user.Sort = num;
                if (num == 1)
                {
                    user.IsAwait = false;
                }
                WFTask task = new WFTask()
                {
                    UnitId = unit.Id,
                    NodeCode = unit.NodeCode,
                    Name = unit.Name,
                    Type = 5,
                    PrevUnitId = preUnitId,
                    PrevUnitName = preUnitName,
                    IsReject = isReject,
                    Token = token,
                    MessageType = unit.MessageType,
                    // 超时设置
                    IsOvertimeMessage = unit.IsOvertimeMessage,
                    OvertimeMessageStart = unit.OvertimeMessageStart,
                    OvertimeMessageInterval = unit.OvertimeMessageInterval,
                    IsOvertimeGo = unit.IsOvertimeGo,
                    OvertimeGo = unit.OvertimeGo,
                    OvertimeMessageType = unit.OvertimeMessageType,
                    IsBatchAudit = unit.IsBatchAudit,
                    // 处理人
                    User = user,
                };
                if (string.IsNullOrEmpty(task.Name))
                {
                    task.Name = "审批处理";
                }

                list.Add(task);

                num++;
            }
            return list;
        }
        /// <summary>
        /// 获取脚本执行任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private WFTask GetScriptTaskList(WorkFlowUnit unit, bool isReject, Guid preUnitId, string preUnitName, string token)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = unit.Name,
                Type = 10,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = isReject,
                Token = token,

                ExecuteType = unit.ExecuteType,
                SqlDb = unit.SqlDb,
                SqlStr = unit.SqlStr,
                SqlStrRevoke = unit.SqlStrRevoke,
                ApiUrl = unit.ApiUrl,
                ApiUrlRevoke = unit.ApiUrlRevoke,
                Ioc = unit.Ioc,
                IocRevoke = unit.IocRevoke,
            };
            if (string.IsNullOrEmpty(task.Name))
            {
                task.Name = "脚本执行";
            }
            return task;
        }
        /// <summary>
        /// 获取子流程执行脚本
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private WFTask GetSubScriptTask(WorkFlowUnit unit, bool isReject, Guid preUnitId, string preUnitName, string token)
        {
            if (
                (unit.ExecuteType == "1" && (string.IsNullOrEmpty(unit.SqlDb) || string.IsNullOrEmpty(unit.SqlStr) || string.IsNullOrEmpty(unit.SqlStrRevoke)))
                || (unit.ExecuteType == "2" && (string.IsNullOrEmpty(unit.Ioc) || string.IsNullOrEmpty(unit.IocRevoke)))
                || (unit.ExecuteType == "3" && (string.IsNullOrEmpty(unit.ApiUrl) || string.IsNullOrEmpty(unit.ApiUrlRevoke)))
                )
            {
                return null;
            }
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = unit.Name,
                Type = 10,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = isReject,
                Token = token,

                ExecuteType = unit.ExecuteType,
                SqlDb = unit.SqlDb,
                SqlStr = unit.SqlStr,
                SqlStrRevoke = unit.SqlStrRevoke,
                ApiUrl = unit.ApiUrl,
                ApiUrlRevoke = unit.ApiUrlRevoke,
                Ioc = unit.Ioc,
                IocRevoke = unit.IocRevoke
            };
            if (string.IsNullOrEmpty(task.Name))
            {
                task.Name = "子流程脚本执行";
            }
            else
            {
                task.Name = $"{unit.Name}_子流程脚本执行";
            }
            return task;
        }

        /// <summary>
        /// 获取子流程执行任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="userList"></param>
        /// <param name="isReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private List<WFTask> GetSubTask(WorkFlowUnit unit, List<WFUserInfo> userList, bool isReject, Guid preUnitId, string preUnitName, string token)
        {
            List<WFTask> list = new List<WFTask>();
            var childProcessId = _guidGenerator.Create();
            foreach (var user in userList)
            {
                WFTask task = new WFTask()
                {
                    UnitId = unit.Id,
                    NodeCode = unit.NodeCode,
                    Name = unit.Name,
                    Type = 3,
                    PrevUnitId = preUnitId,
                    PrevUnitName = preUnitName,
                    IsReject = isReject,
                    Token = token,

                    IsAuto = unit.IsAuto,
                    WfschemeId = unit.WfschemeId,
                    WfVersionId = unit.WfVersionId,

                    ExecuteType = unit.ExecuteType,
                    SqlDb = unit.SqlDb,
                    SqlStr = unit.SqlStr,
                    SqlStrRevoke = unit.SqlStrRevoke,
                    ApiUrl = unit.ApiUrl,
                    ApiUrlRevoke = unit.ApiUrlRevoke,
                    Ioc = unit.Ioc,
                    IocRevoke = unit.IocRevoke,

                    // 处理人
                    User = user,

                    ChildProcessId = user.ChildProcessId
                };
                if (string.IsNullOrEmpty(task.Name))
                {
                    task.Name = "子流程创建";
                }
                if (!task.ChildProcessId.IsEmpty())
                {
                    task.ChildProcessId = childProcessId;
                }
                list.Add(task);
            }
            return list;
        }

        /// <summary>
        /// 获取成立的条件ID
        /// </summary>
        /// <param name="conditions">条件列表</param>
        /// <returns></returns>
        private async Task<List<string>> GetConditionIds(List<WorkFlowCondition> conditions)
        {
            List<string> list = new List<string>();
            foreach (var condition in conditions)
            {
                if (condition.Type == "1")// 字段比较
                {
                    string sql = string.Format("select {0} from {1} where {2} = @processId ", condition.Cfield, condition.Table, condition.Rfield, condition.Rfield);
                    // { processId = _config.Params.ProcessId }
                    var parameters = new DynamicParameters();
                    parameters.Add("@processId", _config.Params.ProcessId, DbType.Guid);
                    //集成DAPPER查询,尽量去掉使用FINDTABLE这种形式
                    var query = await _sqlExecutor.QueryAsync<dynamic>(sql, parameters );
 
                    var dt = await _dataBaseService.FindTable(
                        sql,
                        new Dictionary<string, string>
                        {
                            { "processId", _config.Params.ProcessId.ToString() }
                        }
                    );
                     
                    if (query.Any())//dt.Rows.Count > 0
                    {
                        var value = (query.FirstOrDefault() as IDictionary<string, object>)[condition.Cfield].ToString();
                        //var value = dt.Rows[0][condition.Cfield.ToLower()].ToString();

                        switch (condition.CompareType)
                        {
                            case "1":
                                if (value == condition.Value)
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "2":
                                if (value != condition.Value)
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "3":
                                if (Convert.ToDecimal(value) > Convert.ToDecimal(condition.Value))
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "4":
                                if (Convert.ToDecimal(value) >= Convert.ToDecimal(condition.Value))
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "5":
                                if (Convert.ToDecimal(value) < Convert.ToDecimal(condition.Value))
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "6":
                                if (Convert.ToDecimal(value) <= Convert.ToDecimal(condition.Value))
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "7":
                                if (value.IndexOf(condition.Value) != -1)
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "8":
                                if (value.IndexOf(condition.Value) == -1)
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "9":
                                if (condition.Value.IndexOf(value) != -1)
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                            case "10":
                                if (condition.Value.IndexOf(value) == -1)
                                {
                                    list.Add(condition.Code);
                                }
                                break;
                        }
                    }
                }
                else
                {// sql语句
                    var dt = await _dataBaseService.FindTable(condition.Sql, new { processId = _config.Params.ProcessId, userId = CreateUser.Id, userAccount = CreateUser.Account, companyId = CreateUser.CompanyId, departmentId = CreateUser.DepartmentId });
                    if (dt.Rows.Count > 0)
                    {
                        list.Add(condition.Code);
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 判断流程审批规则-是否自动同意
        /// </summary>
        /// <param name="auditUsers">审批处理人</param>
        /// <param name="atuoAgrees">自动同意策略1.处理人就是提交人 2.处理人和上一步的处理人相同 3.处理人审批过</param>
        /// <param name="isReject">是否是驳回</param>
        /// <returns></returns>
        private bool IsAtuoAgree(List<WFUserInfo> auditUsers, string[] atuoAgrees, bool isReject)
        {
            if (isReject)
            {
                return false;
            }
            bool res = false;
            if (auditUsers.Count == 0 || atuoAgrees.Length == 0)
            {
                return res;
            }
            foreach (var item in atuoAgrees)
            {
                switch (item)
                {
                    case "1"://处理人就是提交人
                        if (auditUsers.FindIndex(t => t.Id == CreateUser.Id) != -1)
                        {
                            res = true;
                        }
                        break;
                    case "2"://处理人和上一步的处理人相同
                        if (auditUsers.FindIndex(t => t.Id == CurrentUser.Id) != -1)
                        {
                            res = true;
                        }
                        break;
                    case "3"://处理人审批过
                        if (auditUsers.FindIndex(t => t.IsAgree) != -1 && !isReject)
                        {
                            res = true;
                        }
                        break;
                }
            }
            return res;
        }

        /// <summary>
        /// 获取开始任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="IsReject"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private WFTask GetStartTask(WorkFlowUnit unit, bool IsReject, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                Name = unit.Name,
                Token = _guidGenerator.Create().ToString(),
                Type = 4,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
                IsReject = IsReject,
                MessageType = unit.MessageType,
                User = _config.Params.CreateUser,
                NodeCode = unit.NodeCode,
            };
            return task;
        }
        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private WFTask GetEndTask(WorkFlowUnit unit, Guid preUnitId, string preUnitName)
        {
            WFTask task = new WFTask()
            {
                UnitId = unit.Id,
                NodeCode = unit.NodeCode,
                Name = "流程结束",
                Token = _guidGenerator.Create().ToString(),
                Type = 100,
                PrevUnitId = preUnitId,
                PrevUnitName = preUnitName,
            };
            return task;
        }
        /// <summary>
        /// 结束节点消息
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="userList"></param>
        /// <param name="preUnitId"></param>
        /// <param name="preUnitName"></param>
        /// <returns></returns>
        private List<WFTask> GetEndMsgTask(WorkFlowUnit unit, List<WFUserInfo> userList, Guid preUnitId, string preUnitName)
        {
            List<WFTask> list = new List<WFTask>();
            foreach (var user in userList)
            {

                WFTask task = new WFTask()
                {
                    UnitId = unit.Id,
                    NodeCode = unit.NodeCode,
                    Name = "流程结束",
                    Token = _guidGenerator.Create().ToString(),
                    Type = 101,
                    PrevUnitId = preUnitId,
                    PrevUnitName = preUnitName,
                    MessageType = unit.MessageType,

                    // 处理人(消息通知人)
                    User = user
                };
                list.Add(task);
            }
            return list;
        }

    }
}
