using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
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
    public class GetCompanyPhonePagedRequestInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 所属公司Id
        /// </summary>
        public Guid? companyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid? DepartmentId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        public int? EmployeeType { get; set; }
        /// <summary>
        /// 办理状态
        /// </summary>
        public int? Status { get; set; }


        public string Filter { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime desc";
            }

            Filter = Filter?.Trim();
        }
    }
}
