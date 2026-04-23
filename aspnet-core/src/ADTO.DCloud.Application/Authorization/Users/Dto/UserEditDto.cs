using System;
using System.ComponentModel.DataAnnotations;
using ADTO.DCloud.Organizations.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using Stripe.Identity;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    //Mapped to/from User in CustomDtoMapper
    [AutoMap(typeof(User))]
    public class UserEditDto : IPassivable
    {
        /// <summary>
        ///  设置null创建新用户。设置用户Id以更新用户
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(ADTOSharpUserBase.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(ADTOSharpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }
        /// <summary>
        /// 邮件地址
        /// </summary>
        [EmailAddress]
        [StringLength(ADTOSharpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        [StringLength(ADTOSharpUserBase.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 密码,如果不修改密码,设为""
        /// </summary>
        // Not used "Required" attribute since empty value is used to 'not change password'
        [StringLength(ADTOSharpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 下次登录必须更改密码
        /// </summary>
        public bool ShouldChangePasswordOnNextLogin { get; set; }
        /// <summary>
        /// 是否开启二次验证
        /// </summary>
        public virtual bool IsTwoFactorEnabled { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public virtual bool IsLockoutEnabled { get; set; }

        /// <summary>
        /// 部门,默认部门,在维护用户的部门时,默认第一条记录为默认部门,可以修改任一记录为默认部门
        /// </summary>
        public virtual  Guid? DepartmentId { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public virtual Guid? CompanyId { get; set; }

        /// <summary>
        /// 直属上级
        /// </summary>
        public Guid? ManagerId { get; set; }

        /// <summary>
        /// 直属上级
        /// </summary>
        public string ManagerName { get; set; }
    }
}