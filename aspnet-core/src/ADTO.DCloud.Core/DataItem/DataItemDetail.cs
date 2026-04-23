using System;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ADTO.DCloud.DataItem
{
    /// <summary>
    /// 字典详情
    /// </summary>
    [Description("字典详情"), Table("DataItemDetails")]
    public class DataItemDetail : FullAuditedEntity<Guid>, IMayHaveTenant, IPassivable, IRemark, IDisplayOrder
    {
        /// <summary>
        /// 分类主键Id
        /// </summary>
        [ForeignKey("DataItem")]
        [Description("分类主键Id")]
        public Guid ItemId { get; set; }
        public virtual DataItem Item { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        [ForeignKey("DataItemDetail")]
        [Description("父级Id")]
        public Guid? ParentId { get; set; }
        public virtual DataItemDetail? Parent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string ItemName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Description("值")]
        public string ItemValue { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        [Description("租户")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        [Description("排序值")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Description("是否有效")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }
    }
}
