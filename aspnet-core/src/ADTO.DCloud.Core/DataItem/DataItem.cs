using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.DataItem
{
    /// <summary>
    /// 数据字典分类表
    /// </summary>
    [Description("数据字典分类表"), Table("DataItems")]
    public class DataItem : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IPassivable, IRemark, IDisplayOrder
    {
        /// <summary>
        /// 父级Id
        /// </summary>
        [ForeignKey("DataItem")]
        [Description("父级Id")]
        public Guid? ParentId { get; set; }
        public virtual DataItem? Parent { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        [Description("分类编码")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Description("分类名称")]
        public string ItemName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Description("是否有效")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        [Description("租户")]
        public Guid? TenantId { get; set; }
    }
}
