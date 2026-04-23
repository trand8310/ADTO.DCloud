using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;

namespace ADTO.DCloud.WorkFlow.Tasks.Dto
{
    [AutoMap(typeof(WorkFlowTaskLog))]
    public class WorkFlowTaskLogDto : EntityDto<Guid>
    {
        #region 实体成员 
        /// <summary> 
        /// 流程进程主键 
        /// </summary> 
        [Description("流程进程主键")]
        public Guid ProcessId { get; set; }
        /// <summary>
        /// 子流程进程主键
        /// </summary>
        [Description("子流程进程主键")]
        public Guid? ChildProcessId { get; set; }
        /// <summary> 
        /// 流程任务主键 
        /// </summary> 
        [Description("流程任务主键")]
        public Guid? TaskId { get; set; }
        /// <summary>
        /// 任务 token
        /// </summary>
        [Description("任务 token")]
        [StringLength(50)]
        public string Token { get; set; }
        /// <summary> 
        /// 操作码 create创建 agree 同意 disagree 不同意 timeout 超时 
        /// </summary>
        [Description("操作码 create创建 agree 同意 disagree 不同意 timeout 超时")]
        [StringLength(50)]
        public string OperationCode { get; set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        [Description("操作名称")]
        [StringLength(50)]
        public string OperationName { get; set; }
        /// <summary> 
        /// 0.创建流程
        /// 1.审批任务
        /// 2.传阅任务
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
        /// 98 催办任务
        /// 99 脚本执行异常任务
        /// 100 脚本执行任务正常
        /// 101 转办任务
        /// 200 归档
        /// </summary>
        [Description("0.创建流程1.审批任务2.传阅任务3.子流程4.重新创建5.会签任务6 加签审批7 沟通审批10.脚本任务21.等待任务（系统自己完成）22.取消等待任务23.找不到审批人直接跳过24.自动审批规则跳过25.会签任务记录98 催办任务99 脚本执行异常任务100 脚本执行任务正常101 转办任务200 归档")]
        public int? TaskType { get; set; }
        /// <summary>
        /// 审批是否同意 1 是 0 不是
        /// </summary>
        [Description("审批是否同意 1 是 0 不是")]
        public int? IsAgree { get; set; }
        /// <summary> 
        /// 流程节点ID 
        /// </summary>
        [Description("流程节点ID")]
        public Guid? UnitId { get; set; }
        /// <summary>
        /// 流程节点名称 
        /// </summary>
        [Description("流程节点名称")]
        [StringLength(50)]
        public string UnitName { get; set; }
        /// <summary> 
        /// 上一流程节点ID 
        /// </summary> 
        [Description("上一流程节点ID")]
        public Guid? PrevUnitId { get; set; }
        /// <summary> 
        /// 上一流程节点名称 
        /// </summary> 
        [Description("上一流程节点名称")]
        [StringLength(50)]
        public string PrevUnitName { get; set; }
        /// <summary>
        /// 任务创建时间
        /// </summary>
        [Description("任务创建时间")]
        public DateTime? TaskCreateDate { get; set; }
        /// <summary>
        /// 任务人Id
        /// </summary>
        [Description("任务人Id")]
        public Guid UserId { get; set; }
        /// <summary>
        /// 任务人名称
        /// </summary>
        [Description("任务人名称")]
        [StringLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// 执行人岗位
        /// </summary>
        [Description("执行人岗位")]
        public Guid? PostId { get; set; }

        /// <summary> 
        /// 备注信息
        /// </summary> 
        [Description("备注信息")]
        [StringLength(200)]
        public string Remark { get; set; }
        /// <summary> 
        /// 盖章图片
        /// </summary> 
        [Description("盖章图片")]
        [StringLength(50)]
        public string StampImg { get; set; }
        /// <summary>
        /// 审批附件
        /// </summary>
        [Description("审批附件")]
        public string FileId { get; set; }
        /// <summary>
        /// 是否允许撤销 1 允许 0 不允许
        /// </summary>
        [Description("是否允许撤销 1 允许 0 不允许")]
        public int? IsCancel { get; set; }
        /// <summary>
        /// 是否是最近一次处理 1.是 0 不是
        /// </summary>
        [Description("是否是最近一次处理 1.是 0 不是")]
        public int? IsLast { get; set; }

        /// <summary>
        /// 审批要点值
        /// </summary>
        [Description("审批要的值")]
        [StringLength(50)]
        public string AuditTag { get; set; }
        #endregion
        #region 多租户

        /// <summary>
        /// 多租户
        /// </summary>
        [Description("多租户")]
        public Guid? TenantId { get; set; }
        [Description("创建人")]
        public Guid? CreatorUserId { get; set; }
        [Description("创建时间")]
        public DateTime CreationTime { get; set; }
        #endregion
    }
}
