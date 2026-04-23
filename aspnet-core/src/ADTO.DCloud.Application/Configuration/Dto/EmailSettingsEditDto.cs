
using ADTOSharp.Auditing;

namespace ADTO.DCloud.Configuration.Dto
{
    /// <summary>
    /// 邮箱设置选项
    /// </summary>
    public class EmailSettingsEditDto
    {
        /// <summary>
        /// 发送地址
        /// </summary>
        public string DefaultFromAddress { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DefaultFromDisplayName { get; set; }
        /// <summary>
        /// 主机地址
        /// </summary>
        public string SmtpHost { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int SmtpPort { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string SmtpUserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [DisableAuditing]
        public string SmtpPassword { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string SmtpDomain { get; set; }
        /// <summary>
        /// 是否启用SSL
        /// </summary>
        public bool SmtpEnableSsl { get; set; }
        /// <summary>
        /// 是否使用默认授权
        /// </summary>
        public bool SmtpUseDefaultCredentials { get; set; }
    }
}