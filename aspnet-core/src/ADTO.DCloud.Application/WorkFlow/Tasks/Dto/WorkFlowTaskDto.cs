using ADTO.DCloud.WorkFlow.Processes;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.WorkFlow.Tasks.Dto
{

    [AutoMap(typeof(WorkFlowTask))]
    public class WorkFlowTaskDto:CreationAuditedEntityDto<Guid>
    {
        #region 实体成员 
        /// <summary> 
        /// 1.审批任务
        /// 2传阅任务
        /// 3.子流程
        /// 4.重新创建
        /// 5.会签任务
        /// 6 加签审批
        /// 7 沟通审批
        /// 10.脚本任务
        /// 21.等待任务（系统自己完成）
        /// 22.取消等待任务
        /// 23.找不到审批人直接跳过
        /// 24.自动审批规则跳过
        /// 25.会签任务记录
        /// </summary> 
        [Description("1.审批任务2传阅任务3.子流程4.重新创建5.会签任务6 加签审批7 沟通审批10.脚本任务21.等待任务（系统自己完成）22.取消等待任务23.找不到审批人直接跳过24.自动审批规则跳过25.会签任务记录")]
        public int? Type { get; set; }
        /// <summary> 
        /// 流程实例主键 
        /// </summary>
        [Description("流程实例主键")]
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 流程模板编码
        /// </summary>
        [Description("流程模板编码")]
        [StringLength(50)]
        public string ProcessCode { get; set; }
        /// <summary>
        /// 流程实例标题
        /// </summary>
        [Description("流程实例标题")]
        [StringLength(200)]
        public string ProcessTitle { get; set; }
        /// <summary>
        /// 子流程进程主键
        /// </summary>
        [Description("子流程进程主键")]
        public Guid? ChildProcessId { get; set; }
        /// <summary>
        /// 子流程模板ID
        /// </summary>
        [Description("子流程模板ID")]
        public Guid? ChildSchemeId { get; set; }
        /// <summary>
        /// 流程提交人id
        /// </summary>
        [Description("流程提交人id")]
        public Guid ProcessUserId { get; set; }
        /// <summary>
        /// 流程提交人名称
        /// </summary>
        [Description("流程提交人名称")]
        [StringLength(50)]
        public string ProcessUserName { get; set; }
        /// <summary>
        /// 任务令牌；同一节点生成相同的任务令牌同一次流转中
        /// </summary>
        [Description("任务令牌；同一节点生成相同的任务令牌同一次流转中")]
        [StringLength(50)]
        public string Token { get; set; }
        /// <summary> 
        /// 流程节点ID 
        /// </summary> 
        [Description("流程节点ID")]
        public Guid UnitId { get; set; }
        /// <summary> 
        /// 流程节点名称 
        /// </summary>
        [Description("流程节点名称")]
        [StringLength(50)]
        public string UnitName { get; set; }
        /// <summary> 
        /// 上一个任务节点Id 
        /// </summary> 
        [Description("上一个任务节点Id")]
        public Guid PrevUnitId { get; set; }
        /// <summary> 
        /// 上一个节点名称 
        /// </summary> 
        [Description("上一个节点名称")]
        [StringLength(50)]
        public string PrevUnitName { get; set; }
        /// <summary>
        /// 上一个任务的token
        /// </summary>
        [Description("上一个任务的token")]
        [StringLength(50)]
        public string PrevToken { get; set; }
        /// <summary>
        /// 上一个任务的Id
        /// </summary>
        [Description("上一个任务的Id")]
        public string PrevTaskId { get; set; }

        /// <summary>
        /// 触发状态更新的任务ID
        /// </summary>
        [Description("触发状态更新的任务ID")]
        public Guid? UpdateTaskId { get; set; }

        /// <summary>
        /// 任务执行人
        /// </summary>
        [Description("任务执行人")]
        public Guid? UserId { get; set; }
        /// <summary>
        /// 任务执行人名称
        /// </summary>
        [Description("任务执行人名称")]
        [StringLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// 任务执行人部门id
        /// </summary>
        [Description("任务执行人部门id")]
        public Guid? UserDepartmentId { get; set; }
        /// <summary>
        /// 任务执行人公司Id
        /// </summary>
        [Description("任务执行人公司Id")]
        public Guid? UserCompanyId { get; set; }
        /// <summary> 
        /// 任务状态 1.激活 2.待激活 3.完成 4.关闭 5.加签状态 6.转办给其他人 7.作废 8.子流程运行中 9 沟通
        /// </summary>
        [Description("任务状态 1.激活 2.待激活 3.完成 4.关闭 5.加签状态 6.转办给其他人 7.作废 8.子流程运行中 9 沟通")]
        public int? State { get; set; }
        /// <summary>
        /// 是否同意 0 不同意 1 同意（不是驳回的操作都属于同意，具体执行操作看任务日志）  2 没有做选择
        /// </summary>
        [Description("是否同意 0 不同意 1 同意（不是驳回的操作都属于同意，具体执行操作看任务日志）  2 没有做选择")]
        public int? IsAgree { get; set; }
        /// <summary>
        /// 是否自动执行任务
        /// </summary>
        [Description("是否自动执行任务")]
        public int? IsAuto { get; set; }
        /// <summary>
        /// 是否是最近的任务 1.是 0 不是
        /// </summary>
        [Description("是否是最近的任务 1.是 0 不是")]
        public int? IsLast { get; set; }
        /// <summary>
        /// 任务排序
        /// </summary>
        [Description("任务排序")]
        public int? Sort { get; set; }
        /// <summary> 
        /// 任务超时流转到下一个节点时间 
        /// </summary>
        [Description("任务超时流转到下一个节点时间")]
        public DateTime? TimeoutAction { get; set; }
        /// <summary> 
        /// 任务超时提醒消息时间 
        /// </summary>
        [Description("任务超时提醒消息时间")]
        public DateTime? TimeoutNotice { get; set; }
        /// <summary> 
        /// 任务超时消息提醒间隔时间 
        /// </summary>
        [Description("任务超时消息提醒间隔时间")]
        public int? TimeoutInterval { get; set; }
        /// <summary> 
        /// 任务超时消息发送策略编码 
        /// </summary> 
        [Description("任务超时消息发送策略编码")]
        [StringLength(50)]
        public string TimeoutStrategy { get; set; }

        /// <summary> 
        /// 是否被催办 1 被催办了
        /// </summary> 
        [Description("是否被催办 1 被催办了")]
        public int? IsUrge { get; set; }
        /// <summary>
        /// 催办时间
        /// </summary>
        [Description("催办时间")]
        public DateTime? UrgeTime { get; set; }
        /// <summary> 
        /// 加签情况下最初的任务ID
        /// </summary> 
        [Description("加签情况下最初的任务ID")]
        public Guid FirstId { get; set; }
        /// <summary>
        /// 批量审批 1是允许 其他值都不允许
        /// </summary>
        [Description("批量审批 1是允许 其他值都不允许")]
        public int? IsBatchAudit { get; set; }
        /// <summary>
        /// 是否是驳回生成的任务
        /// </summary>
        [Description("是否是驳回生成的任务")]
        public int? IsReject { get; set; }

        /// <summary>
        /// 是否已读 1 是 0 不是
        /// </summary>
        [Description("是否已读 1 是 0 不是")]
        public int? ReadStatus { get; set; }
        /// <summary>
        /// 已读时间
        /// </summary>
        [Description("已读时间")]
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        [Description("是否置顶")]
        public int? IsUp { get; set; }
        /// <summary>
        /// 是否置顶（已办置顶）
        /// </summary>
        [Description("是否置顶（已办置顶）")]
        public int? IsUp2 { get; set; }
        /// <summary>
        /// 审批步骤
        /// </summary>
        [Description("审批步骤")]
        public int? Step { get; set; }
        /// <summary>
        /// 审批时长
        /// </summary>
        [Description("审批时长")]
        public int? AuditTime { get; set; }
        /// <summary>
        /// 是否原路返回（驳回再审批）
        /// </summary>
        [Description("是否原路返回（驳回再审批）")]
        public int? IsRejectBackOld { get; set; }

        /// <summary>
        /// 节点编码
        /// </summary>
        [Description("节点编码")]
        [StringLength(50)]
        public string NodeCode { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public  string CreatorUserName { get; set; }
        #endregion

        #region 扩展属性
        /// <summary> 
        /// 任务完成动作 
        /// </summary> 
        public string OperationName { get; set; }
        /// <summary> 
        /// 任务完成人ID 
        /// </summary> 
        public string MakeUserId { get; set; }
        /// <summary>
        /// 任务完成时间
        /// </summary>
        public DateTime? MakeTime { get; set; }
        /// <summary> 
        /// 是否允许撤销 
        /// </summary> 
        public int? IsCancel { get; set; }
        /// <summary> 
        /// 流程进程是否结束1是0不是 
        /// </summary> 
        public int? IsFinished { get; set; }
        /// <summary> 
        /// 流程是否被作废，3是
        /// </summary> 
        public int? IsDelete { get; set; }

        /// <summary>
        /// 流程创建人
        /// </summary>
        public Guid? PorcessUserId { get; set; }

        /// <summary> 
        /// 流程模板名称 
        /// </summary> 
        public string SchemeName { get; set; }

        /// <summary>
        /// 流程重要级别
        /// </summary>
        public int? Level { get; set; }
        /// <summary> 
        /// 当前处理节点名称 
        /// </summary> 
        public string UnitNames { get; set; }


        #endregion
    }
}
