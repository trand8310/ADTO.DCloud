using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Customers
{
    /// <summary>
    /// 客户跟进记录表
    /// </summary>
    [Description("客户跟进记录表"), Table("CustomerFollowRecords")]
    public class CustomerFollowRecord : FullAuditedAggregateRoot<Guid>, IRemark, IMayHaveTenant
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("Customer")]
        [Description("所属客户Id")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// 客户跟进方式 字典
        /// </summary>
        [Description("客户跟进方式")]
        public string CustomerFollowType { get; set; }

        /// <summary>
        /// 客户跟进阶段 字段
        /// </summary>
        [Description("客户跟进阶段")]
        public string CustomerFollowStage { get; set; }

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
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }
    }
}
