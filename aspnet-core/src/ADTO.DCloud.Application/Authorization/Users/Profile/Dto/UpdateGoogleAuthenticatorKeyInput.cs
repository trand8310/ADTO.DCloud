using System.Collections.Generic;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 更新谷歌登录KEY
    /// </summary>
    public class UpdateGoogleAuthenticatorKeyInput
    {
        public string GoogleAuthenticatorKey { get; set; }
        public string AuthenticatorCode { get; set; }
    }
}
