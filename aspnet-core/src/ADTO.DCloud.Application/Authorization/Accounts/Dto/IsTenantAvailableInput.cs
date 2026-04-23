using System.ComponentModel.DataAnnotations;
using ADTOSharp.MultiTenancy;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class IsTenantAvailableInput
{
    [Required]
    [MaxLength(ADTOSharpTenantBase.MaxTenancyNameLength)]
    public string TenancyName { get; set; }
}