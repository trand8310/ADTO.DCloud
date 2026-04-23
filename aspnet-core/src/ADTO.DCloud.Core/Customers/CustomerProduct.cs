using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Customers
{
    /// <summary>
    /// 客户产品表
    /// </summary>
    [Description("客户产品表"), Table("CustomerProducts")]
    public class CustomerProduct : FullAuditedAggregateRoot<Guid>, IRemark, IMayHaveTenant
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("Customer")]
        [Description("所属客户Id")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Description("产品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 产品规格
        /// </summary>
        [Description("产品规格")]
        public string Spec { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 年销售额
        /// </summary>
        [Description("年销售额")]
        public decimal SalesVolume { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}
