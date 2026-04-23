using ADTOSharp.Runtime.Validation;
using System;



namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class UserLockOutSettingsEditDto: ICustomValidate
    {
        /// <summary>
        /// 开启状态
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 超过多少次失败登录后锁定帐号
        /// </summary>
        public int? MaxFailedAccessAttemptsBeforeLockout { get; set; }

 
        public int? DefaultAccountLockoutSeconds { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!MaxFailedAccessAttemptsBeforeLockout.HasValue)
            {
                throw new ArgumentNullException(nameof(MaxFailedAccessAttemptsBeforeLockout));
            }
            
            if (!DefaultAccountLockoutSeconds.HasValue)
            {
                throw new ArgumentNullException(nameof(DefaultAccountLockoutSeconds));
            }
        }
    }
}