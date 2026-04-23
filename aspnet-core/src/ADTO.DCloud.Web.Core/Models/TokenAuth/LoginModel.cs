using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Models.TokenAuth;

public class LoginModel
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
}
