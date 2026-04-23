using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DeptRoles.Dto
{
    /// <summary>
    /// 修改部门角色状态Dto
    /// </summary>
    public class UpdateStateDeptRoleDto:EntityDto<Guid>
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
    }
}
