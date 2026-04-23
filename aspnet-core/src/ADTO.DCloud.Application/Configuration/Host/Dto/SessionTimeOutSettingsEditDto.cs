using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Configuration.Host.Dto
{
    /// <summary>
    /// 会话超时选项
    /// </summary>
    public class SessionTimeOutSettingsEditDto
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 超时时长,秒
        /// </summary>
        [Range(10, int.MaxValue)]
        public int TimeOutSecond { get; set; }
        /// <summary>
        /// 消息显示时长
        /// </summary>
        [Range(10, int.MaxValue)]
        public int ShowTimeOutNotificationSecond { get; set; }
        /// <summary>
        /// 锁屏超时时长
        /// </summary>
        public bool ShowLockScreenWhenTimedOut { get; set; }
    }
}
