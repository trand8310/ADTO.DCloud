using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Organizations;
using System;

namespace ADTO.DCloud.Authorization.Users.Dto;

[AutoMapFrom(typeof(OrganizationUnit))]
public class UserListOrganizationUnitDto : EntityDto<Guid>
{
    /// <summary>
    /// Ãû³Æ
    /// </summary>
    public string DisplayName { get; set; }
}