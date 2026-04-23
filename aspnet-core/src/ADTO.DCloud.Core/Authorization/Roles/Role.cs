using System.ComponentModel.DataAnnotations;
using ADTOSharp.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using System;
using System.ComponentModel;

namespace ADTO.DCloud.Authorization.Roles;

/// <summary>
/// ½ÇÉ«
/// </summary>
[Description("½ÇÉ«")]
public class Role : ADTOSharpRole<User>
{
    public const int MaxDescriptionLength = 4000;

    public Role()
    {
    }

    public Role(Guid? tenantId, string displayName)
        : base(tenantId, displayName)
    {
    }

    public Role(Guid? tenantId, string name, string displayName)
        : base(tenantId, name, displayName)
    {
    }
    /// <summary>
    /// ÃèÊö
    /// </summary>
    [StringLength(MaxDescriptionLength)]
    public string Description {get; set;}
}
