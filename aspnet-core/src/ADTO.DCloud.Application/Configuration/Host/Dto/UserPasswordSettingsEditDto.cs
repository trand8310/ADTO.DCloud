using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Configuration.Host.Dto
    {/// <summary>
    /// 密码复杂度选项
    /// </summary>
    public class UserPasswordSettingsEditDto
    {
        /// <summary>
        /// 开启密码历史记录,用于修改时比对是否存在过.
        /// </summary>
        public bool EnableCheckingLastXPasswordWhenPasswordChange { get; set; }

        /// <summary>
        /// 检测密码是否与最近的X次是否一样
        /// </summary>
        public int CheckingLastXPasswordCount { get; set; }

        /// <summary>
        /// 开启密码有效期
        /// </summary>
        public bool EnablePasswordExpiration { get; set; }

        /// <summary>
        /// 密码有效天数
        /// </summary>
        public int PasswordExpirationDayCount { get; set; }

        /// <summary>
        /// 密码重置代码有效时长,小时
        /// </summary>
        [Range(1, Int32.MaxValue)]
        public int PasswordResetCodeExpirationHours { get; set; }
    }
}