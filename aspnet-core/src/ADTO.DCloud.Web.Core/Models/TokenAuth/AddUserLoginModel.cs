using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Models.TokenAuth;

public class AddUserLoginModel
{
    ///// <summary>
    ///// 用户ID
    ///// </summary>
    ////public Guid UserId { get; set; }
    /// <summary>
    /// 第三方登录提供者名称
    /// </summary>
    public string LoginProvider { get; set; }

    /// <summary>
    /// 第三方登录唯一KEY
    /// </summary>
    public string ProviderKey { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string DisplayName { get; set; }
}
