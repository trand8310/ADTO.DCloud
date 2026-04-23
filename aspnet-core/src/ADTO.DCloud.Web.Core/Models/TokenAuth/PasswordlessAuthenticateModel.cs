using ADTOSharp.Auditing;

namespace ADTO.DCloud.Models.TokenAuth;

public class PasswordlessAuthenticateModel
{
    /// <summary>
    /// 提供者:EMAIL,SMS,WEICHAT
    /// </summary>
    public string Provider { get; set; }
    /// <summary>
    /// 第三方的唯一值
    /// </summary>
    public string ProviderValue { get; set; }

    /// <summary>
    /// 校验码,系统生成的一个随机码,通常在单位时间内
    /// </summary>
    public string VerificationCode { get; set; }

    /// <summary>
    /// 是否已登录
    /// </summary>
    public bool? SingleSignIn { get; set; }

    public string ReturnUrl { get; set; }
    /// <summary>
    /// 图形验证码.
    /// </summary>
    [DisableAuditing]
    public string CaptchaResponse { get; set; }
}

