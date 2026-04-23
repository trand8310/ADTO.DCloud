using System.Collections.Generic;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 谷歌帐号登录授权KEY
    /// </summary>
    public class GenerateGoogleAuthenticatorKeyOutput
    {
        public string QrCodeSetupImageUrl { get; set; }
        public string GoogleAuthenticatorKey { get; set; }
    }
}
