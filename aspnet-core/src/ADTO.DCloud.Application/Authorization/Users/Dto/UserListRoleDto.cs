using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    [AutoMapFrom(typeof(UserRole))]
    public class UserListRoleDto
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; }
    }
}