using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 修改用户入职审批状态
    /// </summary>
    public class UpdateIsActiveDto
    {
        /// <summary>
        /// 员工Id
        /// </summary>
        public List<Guid> Ids { get; set; }
        /// <summary>
        /// 用户入职状态
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? OutJobDate { get; set; }
    }
}
