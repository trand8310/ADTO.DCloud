using System.ComponentModel.DataAnnotations;
using ADTOSharp.Auditing;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class LinkToUserInput
    {
        public string TenancyName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }
    }
}