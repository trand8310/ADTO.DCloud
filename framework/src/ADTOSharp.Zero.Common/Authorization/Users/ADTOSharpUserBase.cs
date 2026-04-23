using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Configuration;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Extensions;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// Base class for user.
    /// </summary>
    [Table("ADTOSharpUsers")]
    public abstract class ADTOSharpUserBase : FullAuditedEntity<Guid>, IMayHaveTenant, IPassivable
    {
        /// <summary>
        /// Maximum length of the <see cref="UserName"/> property.
        /// </summary>
        public const int MaxUserNameLength = 256;

        /// <summary>
        /// Maximum length of the <see cref="EmailAddress"/> property.
        /// </summary>
        public const int MaxEmailAddressLength = 256;

        /// <summary>
        /// Maximum length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 64;

        /// <summary>
        /// Maximum length of the <see cref="AuthenticationSource"/> property.
        /// </summary>
        public const int MaxAuthenticationSourceLength = 64;

        /// <summary>
        /// UserName of the admin.
        /// admin can not be deleted and UserName of the admin can not be changed.
        /// </summary>
        public const string AdminUserName = "admin";

        /// <summary>
        /// Maximum length of the <see cref="Password"/> property.
        /// </summary>
        public const int MaxPasswordLength = 128;

        /// <summary>
        /// Maximum length of the <see cref="Password"/> without hashed.
        /// </summary>
        public const int MaxPlainPasswordLength = 32;

        /// <summary>
        /// Maximum length of the <see cref="EmailConfirmationCode"/> property.
        /// </summary>
        public const int MaxEmailConfirmationCodeLength = 328;

        /// <summary>
        /// Maximum length of the <see cref="PasswordResetCode"/> property.
        /// </summary>
        public const int MaxPasswordResetCodeLength = 328;

        /// <summary>
        /// Maximum length of the <see cref="PhoneNumber"/> property.
        /// </summary>
        public const int MaxPhoneNumberLength = 32;

        /// <summary>
        /// Maximum length of the <see cref="SecurityStamp"/> property.
        /// </summary>
        public const int MaxSecurityStampLength = 128;

        /// <summary>
        /// Authorization source name.
        /// It's set to external authentication source name if created by an external source.
        /// Default: null.
        /// </summary>
        [StringLength(MaxAuthenticationSourceLength)]
        public virtual string AuthenticationSource { get; set; }

        /// <summary>
        /// 用户名.
        /// 用户名对于其租户必须是唯一的
        /// </summary>
        [Required]
        [StringLength(MaxUserNameLength)]
        public virtual string UserName { get; set; }

        /// <summary>
        /// 该用户的租户Id。
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 用户的电子邮件地址。
        /// 电子邮件地址对于其租户必须是唯一的
        /// </summary>
        [Required]
        [StringLength(MaxEmailAddressLength)]
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// 用户名称,一般存储用户的姓名
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 用户登录密码
        /// </summary>
        [Required]
        [StringLength(MaxPasswordLength)]
        public virtual string Password { get; set; }

        /// <summary>
        /// 电子邮件的确认码。针对有些注册用户,需要启用邮件地址确认时使用
        /// </summary>
        [StringLength(MaxEmailConfirmationCodeLength)]
        public virtual string EmailConfirmationCode { get; set; }

        /// <summary>
        /// 重置密码代码
        /// 如果为空，则无效。
        /// 只用于一次使用，重置后必须设置为空。
        /// </summary>
        [StringLength(MaxPasswordResetCodeLength)]
        public virtual string PasswordResetCode { get; set; }

        /// <summary>
        /// 帐户锁定结束日期
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// 获取或设置访问失败计数
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// 获取或设置已启用的帐户锁定。
        /// </summary>
        public virtual bool IsLockoutEnabled { get; set; }

        /// <summary>
        /// 获取或设置电话号码。
        /// </summary>
        [StringLength(MaxPhoneNumberLength)]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Is the <see cref="PhoneNumber"/> confirmed.
        /// </summary>
        public virtual bool IsPhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the security stamp.
        /// </summary>
        [StringLength(MaxSecurityStampLength)]
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// 是否启用了双因素验证。
        /// </summary>
        public virtual bool IsTwoFactorEnabled { get; set; }

        /// <summary>
        /// 用户的登录列表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserLogin> Logins { get; set; }

        /// <summary>
        /// 用户的角色列表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserRole> Roles { get; set; }

        /// <summary>
        /// 用户的声明列表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserClaim> Claims { get; set; }

        /// <summary>
        /// 用户定义的权限列表
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<UserPermissionSetting> Permissions { get; set; }

        /// <summary>
        /// 用户的偏好设置
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ICollection<Setting> Settings { get; set; }

        /// <summary>
        /// Is the <see cref="ADTOSharpUserBase.EmailAddress"/> confirmed.
        /// </summary>
        public virtual bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// 用户是否为激活状态?
        /// 如果一个用户不是活动的，他/她不能使用该应用程序。
        /// </summary>
        public virtual bool IsActive { get; set; }
        
        protected ADTOSharpUserBase()
        {
            IsActive = true;
            SecurityStamp = SequentialGuidGenerator.Instance.Create().ToString();
        }

        public virtual void SetNewPasswordResetCode()
        {
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(MaxPasswordResetCodeLength);
        }

        public virtual void SetNewEmailConfirmationCode()
        {
            EmailConfirmationCode = Guid.NewGuid().ToString("N").Truncate(MaxEmailConfirmationCodeLength);
        }

        /// <summary>
        /// Creates <see cref="UserIdentifier"/> from this User.
        /// </summary>
        /// <returns></returns>
        public virtual UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, Id);
        }

        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}