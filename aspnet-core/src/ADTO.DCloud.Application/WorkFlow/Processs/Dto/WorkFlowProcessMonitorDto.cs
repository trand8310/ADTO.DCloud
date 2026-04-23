using ADTO.DCloud.WorkFlow.Processes;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程监控
    /// </summary>
    [AutoMap(typeof(WorkFlowProcess))]
    public class WorkFlowProcessMonitorDto : FullAuditedEntityDto<Guid>
    {
        #region 实体成员 
        /// <summary> 
        /// 流程模板主键 
        /// </summary> 
        public Guid? SchemeId { get; set; }
        /// <summary> 
        /// 流程模板编码 
        /// </summary> 
        [StringLength(50)]
        public string SchemeCode { get; set; }
        /// <summary> 
        /// 流程模板名称 
        /// </summary> 
        [StringLength(200)]
        public string SchemeName { get; set; }
        /// <summary> 
        /// 流程进程自定义标题 
        /// </summary> 
        [StringLength(200)]
        public string Title { get; set; }
        /// <summary> 
        /// 流程进程等级 
        /// </summary> 
        public int? Level { get; set; }
        /// <summary> 
        /// 流程进程有效标志 1正常2草稿3作废
        /// </summary> 
        public int? IsActive { get; set; }
        /// <summary> 
        /// 是否重新发起1是0不是 
        /// </summary> 
        public int? IsAgain { get; set; }
        /// <summary> 
        /// 流程进程是否结束1是0不是 
        /// </summary> 
        public int? IsFinished { get; set; }
        /// <summary> 
        /// 是否是子流程进程1是0不是 
        /// </summary> 
        public int? IsChild { get; set; }

        /// <summary> 
        /// 子流程执行方式1异步0同步
        /// </summary> 
        public int? IsAsyn { get; set; }
        /// <summary>
        /// 父流程的发起子流程的节点Id
        /// </summary>
        public Guid? ParentNodeId { get; set; }
        /// <summary> 
        /// 流程进程父进程任务主键 
        /// </summary> 
        public Guid? ParentTaskId { get; set; }
        /// <summary> 
        /// 流程进程父进程主键 
        /// </summary> 
        public Guid? ParentProcessId { get; set; }
        /// <summary>
        /// 1表示开始处理过了 0 还没人处理过
        /// </summary>
        public int? IsStart { get; set; }
        /// <summary>
        /// 是否允许撤销 1 允许 0 不允许
        /// </summary>
        public int? IsCancel { get; set; }
        /// <summary> 
        /// 流程完成时间
        /// </summary> 
        public DateTime? FinishTime { get; set; }
        /// <summary>
        /// 完成处理时长
        /// </summary>
        public int? ProcessMinutes { get; set; }

        /// <summary> 
        /// 创建人名称 
        /// </summary> 
        [StringLength(50)]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 创建人账号
        /// </summary>
        [StringLength(50)]
        public string CreateUserAccount { get; set; }
        /// <summary>
        /// 创建人公司
        /// </summary>
        public Guid? CreateUserCompanyId { get; set; }
        /// <summary>
        /// 创建人部门
        /// </summary>
        public Guid? CreateUserDepartmentId { get; set; }
        /// <summary>
        /// 实际创建人
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 保存表单数据字段
        /// </summary>
        [StringLength(1000)]
        public string Keyword { get; set; }
        /// <summary>
        /// 密级(0公开，1内部，2秘密，3机密，4绝密)
        /// </summary>
        public int? SecretLevel { get; set; }
        /// <summary>
        /// 流程关联字段
        /// </summary>
        public Guid? RprocessId { get; set; }
        /// <summary>
        /// 流程执行有错误
        /// </summary>
        public int? IsException { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public int? Index { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        [StringLength(255)]
        public string Keyword1 { get; set; }
        /// <summary>
        /// 扩展字段2（保存创建流程人岗位id）
        /// </summary>
        [StringLength(255)]
        public string Keyword2 { get; set; }
        /// <summary>
        /// 扩展字段3
        /// </summary>
        [StringLength(255)]
        public string Keyword3 { get; set; }
        /// <summary>
        /// 扩展字段4
        /// </summary>
        [StringLength(255)]
        public string Keyword4 { get; set; }
        /// <summary>
        /// 扩展字段5
        /// </summary>
        [StringLength(255)]
        public string Keyword5 { get; set; }
        /// <summary>
        /// 总体审批时长
        /// </summary>
        public int? AllAuditTime { get; set; }
        #endregion

        #region 扩展字段
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务主键
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public int? TaskType { get; set; }

        /// <summary> 
        /// 是否被催办 1 被催办了
        /// </summary> 
        public int? IsUrge { get; set; }

        /// <summary> 
        /// 审批人
        /// </summary> 
        public string AuditUserIds { get; set; }
        /// <summary>
        /// 流程重要级别
        /// </summary>
        public int? Step { get; set; }
        /// <summary> 
        /// 当前处理节点名称 
        /// </summary> 
        public string UnitNames { get; set; }
        #endregion
    }
}
