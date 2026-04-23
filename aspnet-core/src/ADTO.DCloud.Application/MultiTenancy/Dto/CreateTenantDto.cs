using System;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTOSharp.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.MultiTenancy.Dto;

/// <summary>
/// 新增租户
/// </summary>
[AutoMapTo(typeof(Tenant))]
public class CreateTenantDto
{
    public CreateTenantDto()
    {
        AdminEmailAddress = string.Empty;
    }
    /// <summary>
    /// 租户名称
    /// </summary>
    [Required]
    [StringLength(ADTOSharpTenantBase.MaxTenancyNameLength)]
    [RegularExpression(TenantConsts.TenancyNameRegex)]
    public string TenancyName { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    [StringLength(TenantConsts.MaxNameLength)]
    public string Name { get; set; }

    /// <summary>
    /// 链接字符串
    /// </summary>
    [MaxLength(ADTOSharpTenantBase.MaxConnectionStringLength)]
    //[DisableAuditing]
    public string ConnectionString { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    public bool IsActive { get; set; }

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

    public bool ShouldChangePasswordOnNextLogin { get; set; }

    public bool SendActivationEmail { get; set; }

    public Guid? EditionId { get; set; }



    /// <summary>
    /// 有效期日期
    /// </summary>
    public DateTime? SubscriptionEndDateUtc { get; set; }

    /// <summary>
    /// 是否试用
    /// </summary>
    public bool IsInTrialPeriod { get; set; }
}
