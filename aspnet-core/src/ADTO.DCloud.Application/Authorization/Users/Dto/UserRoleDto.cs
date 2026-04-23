using System;

namespace ADTO.DCloud.Authorization.Users.Dto;

public class UserRoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }
    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }
    /// <summary>
    /// 显示名称
    /// </summary>
    public string RoleDisplayName { get; set; }
    /// <summary>
    /// 是否分配
    /// </summary>
    public bool IsAssigned { get; set; }
    /// <summary>
    /// 是否继承于组织数据
    /// </summary>
    public bool InheritedFromOrganizationUnit { get; set; }
}