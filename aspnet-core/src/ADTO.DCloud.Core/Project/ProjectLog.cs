using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Project
{
    /// <summary>
    /// 项目操作日志
    /// </summary>
    [Description("项目操作日志"), Table("ProjectLogs")]
    public class ProjectLog : Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        /// <summary>
        /// 所属项目
        /// </summary>
        [ForeignKey("ProjectInfo")]
        [Description("所属项目Id")]
        public Guid ProjectId { get; set; }
        public virtual ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 操作类型 字典（新增、修改、分享、审核、跟进 等所有操作）
        /// </summary>
        [Description("操作类型")]
        public string OperateType { get; set; }

        /// <summary>
        /// 数据详情
        /// </summary>
        [Description("数据详情")]
        public string DataDetail { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreationTime { get; set; }
    }
}
