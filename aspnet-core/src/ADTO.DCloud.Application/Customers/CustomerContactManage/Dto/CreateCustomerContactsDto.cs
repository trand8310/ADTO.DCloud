using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Customers.CustomerContactManage.Dto
{
    /// <summary>
    /// 新增客户联系人DTO
    /// </summary>
    [AutoMapTo(typeof(CustomerContacts))]
    public class CreateCustomerContactsDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        [Required(ErrorMessage ="所属客户不能为空")]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [Required(ErrorMessage = "姓名不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Required(ErrorMessage = "联系电话不能为空")]
        public string Phone { get; set; }

        /// <summary>
        /// 联系人角色 字典
        /// </summary>
        [Required(ErrorMessage = "联系人角色不能为空")]
        public string ContactRole { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [Required(ErrorMessage = "职务不能为空")]
        public string Position { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required(ErrorMessage = "联系人邮箱不能为空")]
        public string Email { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        [Required(ErrorMessage = "联系人微信不能为空")]
        public string WeChat { get; set; }

        /// <summary>
        /// 其他账号
        /// </summary>
        public string OtherId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
