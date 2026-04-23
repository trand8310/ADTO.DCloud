using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTOSharp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.MultiTenancy.Dto;

/// <summary>
/// TenantEditDto
/// </summary>
[AutoMap(typeof(Tenant))]
public class TenantEditDto: EntityDto<Guid>
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

    /// <summary>
    /// 管理员名称
    /// </summary>
    [StringLength(ADTOSharpUserBase.MaxNameLength)]
    public string AdminName { get; set; }

    /// <summary>
    /// 管理员邮箱
    /// </summary>
    // [Required]
    [EmailAddress]
    [StringLength(ADTOSharpUserBase.MaxEmailAddressLength)]
    public string AdminEmailAddress { get; set; }

    /// <summary>
    /// 管理员密码
    /// </summary>
    [StringLength(ADTOSharpUserBase.MaxPasswordLength)]
    [DisableAuditing]
    public string AdminPassword { get; set; }
}
