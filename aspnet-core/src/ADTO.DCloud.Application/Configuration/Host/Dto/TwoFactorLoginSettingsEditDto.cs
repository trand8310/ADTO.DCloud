namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class TwoFactorLoginSettingsEditDto
    {
        /// <summary>
        /// 当前应用是否启用
        /// </summary>
        public bool IsEnabledForApplication { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 开启邮件验证
        /// </summary>
        public bool IsEmailProviderEnabled { get; set; }
        /// <summary>
        /// 开启短信验证
        /// </summary>
        public bool IsSmsProviderEnabled { get; set; }
        /// <summary>
        /// 开启记住我选项
        /// </summary>
        public bool IsRememberBrowserEnabled { get; set; }
        /// <summary>
        /// 第三方社交登录验证
        /// </summary>
        public bool IsGoogleAuthenticatorEnabled { get; set; }
    }
}