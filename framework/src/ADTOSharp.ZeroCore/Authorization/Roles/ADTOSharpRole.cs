using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Authorization.Roles;

/// <summary>
/// Represents a role in an application. A role is used to group permissions.
/// </summary>
/// <remarks> 
/// Application should use permissions to check if user is granted to perform an operation.
/// Checking 'if a user has a role' is not possible until the role is static (<see cref="ADTOSharpRoleBase.IsStatic"/>).
/// Static roles can be used in the code and can not be deleted by users.
/// Non-static (dynamic) roles can be added/removed by users and we can not know their name while coding.
/// A user can have multiple roles. Thus, user will have all permissions of all assigned roles.
/// </remarks>
public abstract class ADTOSharpRole<TUser> : ADTOSharpRoleBase, IFullAudited<TUser>
    where TUser : ADTOSharpUser<TUser>
{
    /// <summary>
    /// Maximum length of the <see cref="ConcurrencyStamp"/> property.
    /// </summary>
    public const int MaxConcurrencyStampLength = 128;

    /// <summary>
    /// 角色唯一名称,这个名称如果是字母,则会用大写的方式存储
    /// </summary>
    [Required]
    [StringLength(MaxNameLength)]
    public virtual string NormalizedName { get; set; }

    /// <summary>
    /// 角色拥有的声明
    /// </summary>
    [ForeignKey("RoleId")]
    public virtual ICollection<RoleClaim> Claims { get; set; }

    /// <summary>
    /// 持久化到存储区时，必须更改的随机值,高并发时有用
    /// </summary>
    [StringLength(MaxConcurrencyStampLength)]
    public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// 存储删除记录时操作的用户
    /// </summary>
    public virtual TUser DeleterUser { get; set; }
    /// <summary>
    /// 存储创建这条记录的用户
    /// </summary>
    public virtual TUser CreatorUser { get; set; }
    /// <summary>
    /// 存储最后一次修改这条记录的用户
    /// </summary>
    public virtual TUser LastModifierUser { get; set; }

    protected ADTOSharpRole()
    {
        SetNormalizedName();
    }

    /// <summary>
    /// Creates a new <see cref="ADTOSharpRole{TUser}"/> object.
    /// </summary>
    /// <param name="tenantId">TenantId or null (if this is not a tenant-level role)</param>
    /// <param name="displayName">Display name of the role</param>
    protected ADTOSharpRole(Guid? tenantId, string displayName)
        : base(tenantId, displayName)
    {
        SetNormalizedName();
    }

    /// <summary>
    /// Creates a new <see cref="ADTOSharpRole{TUser}"/> object.
    /// </summary>
    /// <param name="tenantId">TenantId or null (if this is not a tenant-level role)</param>
    /// <param name="name">Unique role name</param>
    /// <param name="displayName">Display name of the role</param>
    protected ADTOSharpRole(Guid? tenantId, string name, string displayName)
        : base(tenantId, name, displayName)
    {
        SetNormalizedName();
    }

    public virtual void SetNormalizedName()
    {
        NormalizedName = Name.ToUpperInvariant();
    }
}