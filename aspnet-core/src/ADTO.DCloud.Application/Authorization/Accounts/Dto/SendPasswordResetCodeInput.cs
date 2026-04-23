using System.ComponentModel.DataAnnotations;
using ADTOSharp.Authorization.Users;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class SendPasswordResetCodeInput
{
    [Required]
    [MaxLength(ADTOSharpUserBase.MaxEmailAddressLength)]
    public string EmailAddress { get; set; }
}