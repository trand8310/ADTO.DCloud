using System.Collections.Generic;

namespace ADTO.DCloud.Authorization.Roles.Dto;

public class GetRolesInput
{
    /// <summary>
    /// 权限列表,可以查出拥有指定权限值的角色
    /// </summary>
    public List<string> Permissions { get; set; }
    /// <summary>
    /// 查询关关键词
    /// </summary>
    public string Keyword { get; set; }
}
