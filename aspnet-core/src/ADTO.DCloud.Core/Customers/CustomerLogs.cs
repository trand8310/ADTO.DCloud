using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Customers
{
    /// <summary>
    /// 客户相关操作日志记录表
    /// </summary>
    [Description("客户日志记录表"), Table("CustomerLogs")]
    public class CustomerLogs : Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("Customer")]
        [Description("所属客户Id")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

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
