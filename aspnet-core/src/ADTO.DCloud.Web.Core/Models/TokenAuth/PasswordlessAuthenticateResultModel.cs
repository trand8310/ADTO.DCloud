using System;

namespace ADTO.DCloud.Models.TokenAuth;


public class PasswordlessAuthenticateResultModel
{
    /// <summary>
    /// 用户登录令牌
    /// </summary>
    public string AccessToken { get; set; }
    /// <summary>
    /// 加密后的令牌,用于在URL中使用
    /// </summary>
    public string EncryptedAccessToken { get; set; }
    /// <summary>
    /// 刷新令牌时使用的凭据
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// 令牌的有效时长(秒)
    /// </summary>
    public int ExpireInSeconds { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public Guid UserId { get; set; }


    public string ReturnUrl { get; set; }
    /// <summary>
    /// 刷新令牌的有效期时长(秒)
    /// </summary>

    public int RefreshTokenExpireInSeconds { get; set; }
}

