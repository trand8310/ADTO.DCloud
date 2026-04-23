using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Authorization.Users;

/// <summary>
/// Represents a user.
/// </summary>
public abstract class ADTOSharpUser<TUser> : ADTOSharpUserBase, IFullAudited<TUser>
    where TUser : ADTOSharpUser<TUser>
{
    /// <summary>
    /// Maximum length of the <see cref="ConcurrencyStamp"/> property.
    /// </summary>
    public const int MaxConcurrencyStampLength = 128;

    /// <summary>
    /// 用户名.
    /// 用户名对于其租户必须是唯一的
    /// </summary>
    [Required]
    [StringLength(MaxUserNameLength)]
    public virtual string NormalizedUserName { get; set; }

    /// <summary>
    /// 用户的电子邮件地址。
    /// 电子邮件地址对于其租户必须是唯一的
    /// </summary>
    [Required]
    [StringLength(MaxEmailAddressLength)]
    public virtual string NormalizedEmailAddress { get; set; }

    /// <summary>
    /// A random value that must change whenever a user is persisted to the store
    /// </summary>
    [StringLength(MaxConcurrencyStampLength)]
    public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    public virtual ICollection<UserToken> Tokens { get; set; }

    public virtual TUser DeleterUser { get; set; }

    public virtual TUser CreatorUser { get; set; }

    public virtual TUser LastModifierUser { get; set; }

    protected ADTOSharpUser()
    {
    }

    public virtual void SetNormalizedNames()
    {
        NormalizedUserName = UserName.ToUpperInvariant();
        NormalizedEmailAddress = EmailAddress.ToUpperInvariant();
    }
}