using System.ComponentModel.DataAnnotations;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTOSharp.Runtime.Validation;
using ADTO.DCloud.Authorization.Users;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize
    {
        [Required]
        [StringLength(ADTOSharpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(ADTOSharpUserBase.MaxNameLength)]
        public string Name { get; set; }


        [Required]
        [EmailAddress]
        [StringLength(ADTOSharpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }
        /// <summary>
        /// ÐÔ±ð
        /// </summary>
        public string Gender { get; set; }

        [Required]
        [StringLength(ADTOSharpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }
    }
}
