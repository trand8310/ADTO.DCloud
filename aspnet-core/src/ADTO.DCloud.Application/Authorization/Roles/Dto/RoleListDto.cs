using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using System;

namespace ADTO.DCloud.Authorization.Roles.Dto;

[AutoMapFrom(typeof(Role))]
public class RoleListDto : EntityDto<Guid>, IHasCreationTime
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public bool IsStatic { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreationTime { get; set; }
}
