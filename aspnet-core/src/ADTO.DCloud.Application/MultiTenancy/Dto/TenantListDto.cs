using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.MultiTenancy.Dto;

/// <summary>
/// 租户列表
/// </summary>
[AutoMap(typeof(Tenant))]
public class TenantListDto : EntityDto<Guid>, IPassivable, IHasCreationTime
{
    public string TenancyName { get; set; }

    public string Name { get; set; }

    public string EditionDisplayName { get; set; }

    //[DisableAuditing]
    public string ConnectionString { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? SubscriptionEndDateUtc { get; set; }

    /// <summary>
    /// 管理员名称
    /// </summary>
    public string AdminName { get; set; }

    /// <summary>
    /// 管理员邮箱
    /// </summary>

    public string AdminEmailAddress { get; set; }

    /// <summary>
    /// 管理员密码
    /// </summary>
    public string AdminPassword { get; set; }

    //public int? EditionId { get; set; }

    //public bool IsInTrialPeriod { get; set; }
}
