using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Authorization.Roles.Dto;

[AutoMapFrom(typeof(Permission))]
public class PermissionDto : EntityDto<Guid>
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }
}
