using ADTO.DCloud.Security;

namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class SecuritySettingsEditDto
    {
        /// <summary>
        /// 只允许单用户登录
        /// </summary>
        public bool AllowOneConcurrentLoginPerUser { get; set; }
        /// <summary>
        /// 用户默认密码复杂度设置
        /// </summary>
        public bool UseDefaultPasswordComplexitySettings { get; set; }
        /// <summary>
        /// 密码复杂度
        /// </summary>
        public PasswordComplexitySetting PasswordComplexity { get; set; }
        /// <summary>
        /// 默认密码复杂度
        /// </summary>
        public PasswordComplexitySetting DefaultPasswordComplexity { get; set; }
        /// <summary>
        /// 用户离开锁定设置
        /// </summary>
        public UserLockOutSettingsEditDto UserLockOut { get; set; }
        /// <summary>
        /// 双重校验,用于密码+手机结合验证,目前还在实验,前端可以不处理此选项
        /// </summary>
        public TwoFactorLoginSettingsEditDto TwoFactorLogin { get; set; }
    }
}