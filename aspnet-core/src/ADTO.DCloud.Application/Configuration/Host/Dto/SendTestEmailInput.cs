using System.ComponentModel.DataAnnotations;
using ADTOSharp.Authorization.Users;

namespace ADTO.DCloud.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        [MaxLength(ADTOSharpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}