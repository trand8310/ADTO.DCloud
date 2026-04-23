using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    [AutoMap(typeof(User))]
    public class CurrentUserProfileEditDto:EntityDto<Guid>
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [StringLength(ADTOSharpUserBase.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(ADTOSharpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required]
        [StringLength(ADTOSharpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [StringLength(ADTOSharpUserBase.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 手机号码是否已确认
        /// </summary>
        public virtual bool IsPhoneNumberConfirmed { get; set; }
        /// <summary>
        /// 时区
        /// </summary>
        public string Timezone { get; set; }
        /// <summary>
        /// 二维码URL
        /// </summary>
        public string QrCodeSetupImageUrl { get; set; }
        /// <summary>
        /// 是否启用谷歌授权(第三方社交登录)
        /// </summary>
        public bool IsGoogleAuthenticatorEnabled { get; set; }
    }
}