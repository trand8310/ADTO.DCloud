using System.Collections.Generic;
using ADTO.DCloud.Authorization.Permissions.Dto;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}