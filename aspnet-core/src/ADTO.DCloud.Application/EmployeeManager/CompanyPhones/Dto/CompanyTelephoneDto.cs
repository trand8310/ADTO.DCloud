using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.CompanyPhones.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class CompanyTelephoneDto : FullAuditedEntityDto<Guid>
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
        /// 归属部门
        /// </summary>
        public Guid? DepartmentId { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>		
        public int? EmployeeType { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>		
        public string EmployeeTypeText { get; set; }

        /// <summary>
        /// 性别
        /// </summary>	
        public int? Gender { get; set; }

        /// <summary>
        /// 公司号码
        /// </summary>
        public string CompanyTelephone { get; set; }
        /// <summary>
        /// 公司号码-办理时间
        /// </summary>
        public string ProcessingDates { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? InJobDate { get; set; }
    }
}
