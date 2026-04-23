using ADTO.DCloud.Authorization.Delegation;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Authorization.Users.Delegation.Dto
{
    [AutoMap(typeof(UserDelegation))]
    public class CreateUserDelegationDto : IValidatableObject
    {
        [Required]
        public Guid TargetUserId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime > EndTime)
            {
                yield return new ValidationResult("StartTime of a user delegation operation can't be bigger than EndTime!");
            }
        }
    }
}