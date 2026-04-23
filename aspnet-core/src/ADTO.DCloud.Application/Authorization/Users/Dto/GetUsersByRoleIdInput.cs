using ADTO.DCloud.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    /// <summary>
    /// 根据角色Id,分页查询用户,请求
    /// </summary>
    public class GetUsersByRoleIdInput 
    {

        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid? RoleId { get; set; }
    }
}
