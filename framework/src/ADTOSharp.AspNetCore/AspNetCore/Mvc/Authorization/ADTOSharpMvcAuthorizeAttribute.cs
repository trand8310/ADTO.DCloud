using System;
using ADTOSharp.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADTOSharp.AspNetCore.Mvc.Authorization;

/// <summary>
/// This attribute is used on an action of an MVC <see cref="Controller"/>
/// to make that action usable only by authorized users. 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ADTOSharpMvcAuthorizeAttribute : AuthorizeAttribute, IADTOSharpAuthorizeAttribute
{
    /// <inheritdoc/>
    public string[] Permissions { get; set; }

    /// <inheritdoc/>
    public bool RequireAllPermissions { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ADTOSharpMvcAuthorizeAttribute"/> class.
    /// </summary>
    /// <param name="permissions">A list of permissions to authorize</param>
    public ADTOSharpMvcAuthorizeAttribute(params string[] permissions)
    {
        Permissions = permissions;
    }
}