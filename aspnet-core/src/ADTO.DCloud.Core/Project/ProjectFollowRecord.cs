using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Project
{
    /// <summary>
    /// 项目跟进记录
    /// </summary>
    [Description("项目跟进记录"), Table("ProjectFollowRecords")]
    public class ProjectFollowRecord : FullAuditedAggregateRoot<Guid>, IRemark, IMayHaveTenant
    {
        /// <summary>
        /// 所属项目
        /// </summary>
        [ForeignKey("ProjectInfo")]
        [Description("所属项目Id")]
        public Guid ProjectId { get; set; }
        public virtual ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 跟进方式 字典
        /// </summary>
        [Description("跟进方式")]
        public string ProjectFollowType { get; set; }

        /// <summary>
        /// 跟进阶段 字段
        /// </summary>
        [Description("跟进阶段")]
        public string ProjectFollowStage { get; set; }

        /// <summary>
        /// 跟进时间
        /// </summary>
        [Description("跟进时间")]
        public DateTime FollowTime { get; set; }

        /// <summary>
        /// 跟进内容
        /// </summary>
        [Description("跟进内容")]
        public string Content { get; set; }

        /// <summary>
        /// 跟进人Id
        /// </summary>
        [Description("跟进人Id")]
        public Guid FollowUserId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }
    }
}
