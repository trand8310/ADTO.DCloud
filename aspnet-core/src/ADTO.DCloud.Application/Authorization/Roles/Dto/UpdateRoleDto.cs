using ADTOSharp.Authorization.Roles;
using ADTO.DCloud.Authorization.Roles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.Authorization.Roles.Dto;

public class UpdateRoleDto : EntityDto<Guid>
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

    public bool IsDefault { get; set; }

    public List<string> GrantedPermissions { get; set; }
}
