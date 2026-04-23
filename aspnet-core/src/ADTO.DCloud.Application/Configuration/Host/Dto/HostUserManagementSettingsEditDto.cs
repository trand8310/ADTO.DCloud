namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class HostUserManagementSettingsEditDto
    {
        /// <summary>
        /// 是否邮箱必须验证后才能登录
        /// </summary>
        public bool IsEmailConfirmationRequiredForLogin { get; set; }

        /// <summary>
        /// 需要手机短信验证
        /// </summary>
        public bool SmsVerificationEnabled { get; set; }
        /// <summary>
        /// 需要COOKIE,该功能,不设选项,这个结合MVC使用,默认为TRUE
        /// </summary>
        public bool IsCookieConsentEnabled { get; set; }
        /// <summary>
        /// 允许用户选择主题
        /// </summary>
        public bool IsQuickThemeSelectEnabled { get; set; }
        /// <summary>
        /// 开启图形验证码登录,默认FALSE
        /// </summary>
        public bool UseCaptchaOnLogin { get; set; }

        /// <summary>
        /// 允许用户使用自定义图像
        /// </summary>
        public bool AllowUsingGravatarProfilePicture { get; set; }
        
        /// <summary>
        /// 会话超时设置
        /// </summary>
        public SessionTimeOutSettingsEditDto SessionTimeOutSettings { get; set; }
        /// <summary>
        /// 密码复杂度设置
        /// </summary>
        public UserPasswordSettingsEditDto UserPasswordSettings { get; set; }
    }
}