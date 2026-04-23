using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.MultiTenancy.Dto;

[AutoMapFrom(typeof(Tenant))]
public class TenantDto : EntityDto<Guid>
{
    [Required]
    [StringLength(ADTOSharpTenantBase.MaxTenancyNameLength)]
    [RegularExpression(ADTOSharpTenantBase.TenancyNameRegex)]
    public string TenancyName { get; set; }

    [Required]
    [StringLength(ADTOSharpTenantBase.MaxNameLength)]
    public string Name { get; set; }

    public bool IsActive { get; set; }
}
