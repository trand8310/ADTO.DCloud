using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Customers
{
    /// <summary>
    /// 客户分享记录表
    /// </summary>
    [Description("客户分享记录表"), Table("CustomerShareRecords")]
    public class CustomerShareRecord :FullAuditedAggregateRoot<Guid>, IMayHaveTenant 
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("Customer")]
        [Description("所属客户Id")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// 分享人
        /// </summary>
        [Description("分享人")]
        public Guid FromUserId { get; set; }

        /// <summary>
        /// 被分享人
        /// </summary>
        [Description("被分享人")]
        public Guid ToUserId { get; set; }

        ///// <summary>
        ///// 是否可用
        ///// </summary>
        //public bool IsActive { get ; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set ; }
    }
}
