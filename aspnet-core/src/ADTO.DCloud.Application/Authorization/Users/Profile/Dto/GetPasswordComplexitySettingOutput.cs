using ADTO.DCloud.Security;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// 密码复杂度设置
    /// </summary>
    public class GetPasswordComplexitySettingOutput
    {
        public PasswordComplexitySetting Setting { get; set; }
    }
}
