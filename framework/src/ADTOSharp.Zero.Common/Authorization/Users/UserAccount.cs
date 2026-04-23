using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// 存储所有用户的帐号信息,这些帐号含第三方登录帐号
    /// </summary>
    [Table("ADTOSharpUserAccounts"), Description("用户帐号")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class UserAccount : FullAuditedEntity<Guid>
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
        /// 租户ID
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual Guid UserId { get; set; }
        /// <summary>
        /// 链接用户ID
        /// </summary>
        public virtual Guid? UserLinkId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(MaxUserNameLength)]
        public virtual string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [StringLength(MaxEmailAddressLength)]
        public virtual string EmailAddress { get; set; }
    }
}