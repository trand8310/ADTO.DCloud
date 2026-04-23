using System.ComponentModel.DataAnnotations;
using ADTOSharp.Auditing;

namespace ADTO.DCloud.Authorization.Users.Profile.Dto
{
    public class ChangePasswordInput
    {
        /// <summary>
        /// µ±Ç°ÃÜÂë
        /// </summary>
        [Required]
        [DisableAuditing]
        public string CurrentPassword { get; set; }
        /// <summary>
        /// ĞÂÃÜÂë
        /// </summary>
        [Required]
        [DisableAuditing]
        public string NewPassword { get; set; }
    }
}