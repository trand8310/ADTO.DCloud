using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Customers
{
    /// <summary>
    /// 客户联系人信息表
    /// </summary>
    [Description("客户联系人信息表"), Table("CustomerContacts")]
    public class CustomerContacts : FullAuditedAggregateRoot<Guid>, IRemark, IMayHaveTenant
    {
        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("Customer")]
        [Description("所属客户Id")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Description("联系人姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Description("联系电话")]
        public string Phone { get; set; }

        /// <summary>
        /// 联系人角色 字典
        /// </summary>
        [Description("联系人角色")]
        public string ContactRole { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [Description("职务")]
        public string Position { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        [Description("微信")]
        public string WeChat { get; set; }

        /// <summary>
        /// 其他账号
        /// </summary>
        [Description("其他账号")]
        public string OtherId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }
    }
}
