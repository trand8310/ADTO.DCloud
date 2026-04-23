using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceTimeRules.Dto
{
    public class GetAttendanceTimeRulePagedInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 办公地点Id
        /// </summary>
        public Guid? LocationId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; } 

        public string Filter { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime asc";
            }
            Filter = Filter?.Trim();
        }
    }
}
