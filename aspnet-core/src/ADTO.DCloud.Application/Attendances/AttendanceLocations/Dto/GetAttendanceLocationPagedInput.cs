using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceLocations.Dto
{
    public class GetAttendanceLocationPagedInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 办公地点
        /// </summary>
        public Guid? LocationId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }

        public string Keyword { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " DisplayOrder asc,CreationTime asc";
            }
            Keyword = Keyword?.Trim();
        }

    }
}
