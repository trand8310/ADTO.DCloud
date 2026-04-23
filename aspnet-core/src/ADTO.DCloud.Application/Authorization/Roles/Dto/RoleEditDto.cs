using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization.Roles;
using ADTO.DCloud.Authorization.Roles;
using System.ComponentModel.DataAnnotations;
using System;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.Authorization.Roles.Dto;
[AutoMap(typeof(Role))]
public class RoleEditDto : EntityDto<Guid?>
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required]
    [StringLength(ADTOSharpRoleBase.MaxNameLength)]
    public string Name { get; set; }
    /// <summary>
    /// 显示名称
    /// </summary>
    [Required]
    [StringLength(ADTOSharpRoleBase.MaxDisplayNameLength)]
    public string DisplayName { get; set; }
    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(Role.MaxDescriptionLength)]
    public string Description { get; set; }
    /// <summary>
    /// 是否系统角色,此类角色,系统初始化时建立,不能删除
    /// </summary>
    public bool IsStatic { get; set; }
    /// <summary>
    /// 是否默认
    /// </summary>
    public bool IsDefault { get; set; }
}