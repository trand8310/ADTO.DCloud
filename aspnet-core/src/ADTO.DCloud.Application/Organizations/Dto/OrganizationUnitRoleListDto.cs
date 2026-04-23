using ADTO.DCloud.Authorization.Roles;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Organizations.Dto
{
    [AutoMapFrom(typeof(Role))]
    public class OrganizationUnitRoleListDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public DateTime AddedTime { get; set; }
    }
}