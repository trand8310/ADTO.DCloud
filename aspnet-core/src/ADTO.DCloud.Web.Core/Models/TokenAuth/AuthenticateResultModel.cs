using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Models.TokenAuth;
/// <summary>
/// 用户登录时返回的模型
/// </summary>
public class AuthenticateResultModel
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
    public bool RequiresTwoFactorVerification { get; set; }
    public IList<string> TwoFactorAuthProviders { get; set; }
    public string TwoFactorRememberClientToken { get; set; }
    /// <summary>
    /// 是否需要重置密码
    /// </summary>
    public bool ShouldResetPassword { get; set; }
    /// <summary>
    /// 密码重置代码,
    /// </summary>
    public string PasswordResetCode { get; set; }
    /// <summary>
    /// 重定向URL
    /// </summary>
    public string ReturnUrl { get; set; }
    /// <summary>
    /// 用户数据的加密存储字段
    /// </summary>
    public string c { get; set; }

}
