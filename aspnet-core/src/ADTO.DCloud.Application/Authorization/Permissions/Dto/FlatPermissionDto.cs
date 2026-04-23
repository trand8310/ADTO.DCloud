using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Authorization.Permissions.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class FlatPermissionDto : EntityDto<Guid>
    {
        public string ParentName { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool IsGrantedByDefault { get; set; }
    }
}