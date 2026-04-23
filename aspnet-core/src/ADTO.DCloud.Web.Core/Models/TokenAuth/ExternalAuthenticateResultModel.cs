using Org.BouncyCastle.Bcpg.OpenPgp;

namespace ADTO.DCloud.Web.Models.TokenAuth;

public class ExternalAuthenticateResultModel
{
    public string AccessToken { get; set; }

    public string EncryptedAccessToken { get; set; }

    public int ExpireInSeconds { get; set; }

    public bool WaitingForActivation { get; set; }

    public string ReturnUrl { get; set; }

    public string RefreshToken { get; set; }

    public int RefreshTokenExpireInSeconds { get; set; }
    /// <summary>
    /// 不确定的用户,一般指第三方登录且未绑定主帐号的用户
    /// </summary>
    public bool UnknownExternalLogin { get; set; }
    public string ProviderKey { get; set; }
}