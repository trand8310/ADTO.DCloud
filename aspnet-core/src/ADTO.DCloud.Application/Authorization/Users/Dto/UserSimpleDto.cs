using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    /// <summary>
    /// 简单用户Dto
    /// </summary>
    public class UserSimpleDto : EntityDto<Guid>
    {
        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户组织架构
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }
    }
}
