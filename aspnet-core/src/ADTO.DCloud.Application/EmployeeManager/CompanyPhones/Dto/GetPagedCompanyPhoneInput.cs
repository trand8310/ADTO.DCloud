using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.CompanyPhones.Dto
{
    public class GetPagedCompanyPhoneInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid? EmployeeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string keyword { get; set; }


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
