using System.Collections.Generic;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 更新谷歌登录KEY的重置代码
    /// </summary>
    public class UpdateGoogleAuthenticatorKeyOutput
    {
        public IEnumerable<string> RecoveryCodes { get; set; }
    }
}
