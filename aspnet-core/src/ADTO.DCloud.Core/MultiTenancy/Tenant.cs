using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Editions;
using ADTO.DCloud.MultiTenancy.Payments;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Timing;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.MultiTenancy
{
    public class Tenant : ADTOSharpTenant<User>
    {
        public const int MaxLogoMimeTypeLength = 64;

        //Can add application specific tenant properties here
        /// <summary>
        /// 系统有效期
        /// </summary>
        public DateTime? SubscriptionEndDateUtc { get; set; }

        /// <summary>
        /// 是否试用
        /// </summary>
        public bool IsInTrialPeriod { get; set; }

        /// <summary>
        /// 客户自定义样式Id
        /// </summary>
        public virtual Guid? CustomCssId { get; set; }

        /// <summary>
        /// 明亮模式Id
        /// </summary>

        public virtual Guid? DarkLogoId { get; set; }

        /// <summary>
        /// 明亮模式文件类型
        /// </summary>
        [MaxLength(MaxLogoMimeTypeLength)]
        public virtual string DarkLogoFileType { get; set; }

        
        public virtual Guid? LightLogoId { get; set; }

        [MaxLength(MaxLogoMimeTypeLength)]
        public virtual string LightLogoFileType { get; set; }

        /// <summary>
        /// 订阅付款类型
        /// </summary>

        public SubscriptionPaymentType SubscriptionPaymentType { get; set; }

        /// <summary>
        /// 管理员名称
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        public string AdminEmailAddress { get; set; }

        /// <summary>
        /// 管理员密码
        /// </summary>
        public string AdminPassword { get; set; }

        protected Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {

        }

        public virtual bool HasLogo()
        {
            return (DarkLogoId != null && DarkLogoFileType != null) ||
                   (LightLogoId != null && LightLogoFileType != null);
        }

        public virtual bool HasDarkLogo()
        {
            return DarkLogoId != null && DarkLogoFileType != null;
        }

        public virtual bool HasLightLogo()
        {
            return LightLogoId != null && LightLogoFileType != null;
        }

        public void ClearDarkLogo()
        {
            DarkLogoId = null;
            DarkLogoFileType = null;
        }

        public void ClearLightLogo()
        {
            LightLogoId = null;
            LightLogoFileType = null;
        }

        public void UpdateSubscriptionDateForPayment(PaymentPeriodType paymentPeriodType, EditionPaymentType editionPaymentType)
        {
            switch (editionPaymentType)
            {
                case EditionPaymentType.NewRegistration:
                case EditionPaymentType.BuyNow:
                    {
                        SubscriptionEndDateUtc = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);
                        break;
                    }
                case EditionPaymentType.Extend:
                    ExtendSubscriptionDate(paymentPeriodType);
                    break;
                case EditionPaymentType.Upgrade:
                    if (HasUnlimitedTimeSubscription())
                    {
                        SubscriptionEndDateUtc = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private void ExtendSubscriptionDate(PaymentPeriodType paymentPeriodType)
        {
            if (SubscriptionEndDateUtc == null)
            {
                throw new InvalidOperationException("Can not extend subscription date while it's null!");
            }

            if (IsSubscriptionEnded())
            {
                SubscriptionEndDateUtc = Clock.Now.ToUniversalTime();
            }

            SubscriptionEndDateUtc = SubscriptionEndDateUtc.Value.AddDays((int)paymentPeriodType);
        }

        private bool IsSubscriptionEnded()
        {
            return SubscriptionEndDateUtc < Clock.Now.ToUniversalTime();
        }

        public int CalculateRemainingHoursCount()
        {
            return SubscriptionEndDateUtc != null
                ? (int)(SubscriptionEndDateUtc.Value - Clock.Now.ToUniversalTime()).TotalHours //converting it to int is not a problem since max value ((DateTime.MaxValue - DateTime.MinValue).TotalHours = 87649416) is in range of integer.
                : 0;
        }

        public bool HasUnlimitedTimeSubscription()
        {
            return SubscriptionEndDateUtc == null;
        }
    }
}
