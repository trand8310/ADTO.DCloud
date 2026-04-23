using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Roles.Dto
{
    public class UsersToRoleInput
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public Guid[] UserIds { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }
    }
}
