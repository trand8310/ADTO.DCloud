using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class UpdateUserPermissionsInput
    {
        public Guid Id { get; set; }

        [Required]
        public List<string> GrantedPermissionNames { get; set; }
    }
}