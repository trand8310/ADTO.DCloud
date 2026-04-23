using System;
using System.Collections.Generic;
using ADTO.DCloud.Dto;



namespace ADTO.DCloud.Common.Dto;

public class FindUsersInput : PagedAndFilteredInputDto
{
    /// <summary>
    /// 租户Id
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 排除当前用户
    /// </summary>
    public bool ExcludeCurrentUser { get; set; }

    /// <summary>
    /// 公司Id
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 用户Id组合
    /// </summary>
    public List<Guid> Ids { get; set; }
}