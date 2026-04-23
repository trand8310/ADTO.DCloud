using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Models.TokenAuth;

public class LoginResultModel
{
    /// <summary>
    /// 登录令牌
    /// </summary>
    public string AccessToken { get; set; }
    /// <summary>
    /// 加密后的登录凭据,使用SIGNALR时,需要
    /// </summary>
    public string EncryptedAccessToken { get; set; }
    /// <summary>
    /// 登录令牌的过期时长
    /// </summary>
    public int ExpireInSeconds { get; set; }
    /// <summary>
    /// 当前用ID
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string RefreshToken { get; set; }
    /// <summary>
    /// 刷新令牌的过期时长
    /// </summary>
    public int RefreshTokenExpireInSeconds { get; set; }

}
