using System;

namespace ADTO.DCloud.Configuration.Host.Dto
{
    /// <summary>
    /// 租户选项
    /// </summary>
    public class TenantManagementSettingsEditDto
    {
        /// <summary>
        /// 开户租户注册
        /// </summary>
        public bool AllowSelfRegistration { get; set; }
        /// <summary>
        /// 新注册的默认租户
        /// </summary>
        public bool IsNewRegisteredTenantActiveByDefault { get; set; }
        /// <summary>
        /// 用户注册时使用图片验证码
        /// </summary>
        public bool UseCaptchaOnRegistration { get; set; }
        /// <summary>
        /// 默认版本号ID,目前只有一个租户
        /// </summary>
        public Guid? DefaultEditionId { get; set; }
    }
}