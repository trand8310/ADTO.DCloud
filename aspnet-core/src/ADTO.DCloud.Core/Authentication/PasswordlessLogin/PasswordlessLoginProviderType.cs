namespace ADTO.DCloud.Authentication.PasswordlessLogin;

public enum PasswordlessLoginProviderType
{
    /// <summary>
    /// 邮件
    /// </summary>
    Email = 1,
    /// <summary>
    /// 短信登录
    /// </summary>
    Sms = 2,
    /// <summary>
    /// 二维码登录
    /// </summary>
    QRCODE = 3,
}
