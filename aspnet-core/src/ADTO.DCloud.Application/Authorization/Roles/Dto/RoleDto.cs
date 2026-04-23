using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization.Roles;
using ADTO.DCloud.Authorization.Roles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace ADTO.DCloud.Authorization.Roles.Dto;

public class RoleDto : EntityDto<Guid>
{
    [Required]
    [StringLength(ADTOSharpRoleBase.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(ADTOSharpRoleBase.MaxDisplayNameLength)]
    public string DisplayName { get; set; }

    public string NormalizedName { get; set; }

    [StringLength(Role.MaxDescriptionLength)]
    public string Description { get; set; }

    public List<string> GrantedPermissions { get; set; }
}