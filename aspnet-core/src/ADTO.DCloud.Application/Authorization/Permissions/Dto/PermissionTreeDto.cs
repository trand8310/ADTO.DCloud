using ADTO.DCloud.Modules.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Authorization.Permissions.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class PermissionTreeDto
    {
        
        public string Name { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Description { get; set; }
        
        public bool IsGrantedByDefault { get; set; }
        public List<PermissionTreeDto> Children { get; set; }

    }
}