using ADTOSharp.Authorization.Users;
using System.ComponentModel.DataAnnotations;
 

namespace ADTO.DCloud.Web.Models.TokenAuth;

public class ExternalAuthenticateModel
{
    /// <summary>
    /// 传入调用GetExternalAuthenticationProviders()得到系统支持的第三方登录:WeixinMiniProgram
    /// </summary>
    [Required]
    [MaxLength(UserLogin.MaxLoginProviderLength)]
    public string AuthProvider { get; set; }
    /// <summary>
    /// 第三方登录时,第三方系统的唯一ID,
    /// </summary>
    //[Required]
    //[MaxLength(UserLogin.MaxProviderKeyLength)]
    public string ProviderKey { get; set; }
    /// <summary>
    /// code码,调用系三方登录时,调用其对应API,得到访问code
    /// </summary>
    [Required]
    public string ProviderAccessCode { get; set; }

    public string ReturnUrl { get; set; }

    public bool? SingleSignIn { get; set; }
}