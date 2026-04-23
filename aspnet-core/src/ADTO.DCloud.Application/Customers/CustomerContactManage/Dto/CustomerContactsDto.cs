using System;
using System.ComponentModel;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Customers.CustomerContactManage.Dto
{
    /// <summary>
    /// 客户联系人
    /// </summary>
    [AutoMap(typeof(CustomerContacts))]
    public class CustomerContactsDto : EntityDto<Guid>
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string CustomerCode { get; set; }

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
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

    }
}
