using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.MultiTenancy.Dto;

public class UpdateTenantDto : EntityDto<Guid>
{
    [Required]
    [StringLength(ADTOSharpTenantBase.MaxTenancyNameLength)]
    public string TenancyName { get; set; }

    [Required]
    [StringLength(TenantConsts.MaxNameLength)]
    public string Name { get; set; }

    [DisableAuditing]
    public string ConnectionString { get; set; }

    public Guid? EditionId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? SubscriptionEndDateUtc { get; set; }

    public bool IsInTrialPeriod { get; set; }
}
