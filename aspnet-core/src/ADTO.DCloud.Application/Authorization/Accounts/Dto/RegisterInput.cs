using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADTO.DCloud.Validation;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Extensions;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class RegisterInput : IValidatableObject
{
    [Required]
    [StringLength(ADTOSharpUserBase.MaxNameLength)]
    public string Name { get; set; }


    [Required]
    [StringLength(ADTOSharpUserBase.MaxUserNameLength)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(ADTOSharpUserBase.MaxEmailAddressLength)]
    public string EmailAddress { get; set; }

    [Required]
    [StringLength(ADTOSharpUserBase.MaxPlainPasswordLength)]
    [DisableAuditing]
    public string Password { get; set; }

    [DisableAuditing]
    public string CaptchaResponse { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!UserName.IsNullOrEmpty())
        {
            if (!UserName.Equals(EmailAddress, StringComparison.OrdinalIgnoreCase) && ValidationHelper.IsEmail(UserName))
            {
                yield return new ValidationResult("Username cannot be an email address unless it's same with your email address !");
            }
        }
    }
}