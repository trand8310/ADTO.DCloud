using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class SendEmailActivationLinkInput
{
    [Required]
    public string EmailAddress { get; set; }
}