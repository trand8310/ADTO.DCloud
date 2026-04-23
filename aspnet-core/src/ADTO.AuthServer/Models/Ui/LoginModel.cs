using ADTOSharp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace ADTO.AuthServer.Models.Ui
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string? TenancyName { get; set; }
    }
}
