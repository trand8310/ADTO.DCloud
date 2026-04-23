using System.ComponentModel.DataAnnotations;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;

namespace ADTO.DCloud.Models.TokenAuth;

/// <summary>
/// 用户登录模型
/// </summary>
public class AuthenticateModel
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [StringLength(ADTOSharpUserBase.MaxUserNameLength)]
    public string UserName { get; set; }

    /// <summary>
    /// 登录密码
    /// </summary>
    [Required]
    [StringLength(ADTOSharpUserBase.MaxPlainPasswordLength)]
    [DisableAuditing]
    public string Password { get; set; }
    /// <summary>
    /// 是否单一登录
    /// </summary>
    public bool? SingleSignIn { get; set; }
    /// <summary>
    /// 是否记住客户凭据
    /// </summary>
    public bool RememberClient { get; set; }
    /// <summary>
    /// 重定向URL
    /// </summary>
    public string ReturnUrl { get; set; }
    /// <summary>
    /// 二次验证码
    /// </summary>
    public string TwoFactorVerificationCode { get; set; }
    /// <summary>
    /// 二次验证凭据
    /// </summary>
    public string TwoFactorRememberClientToken { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    [DisableAuditing]
    public string CaptchaResponse { get; set; }

}
