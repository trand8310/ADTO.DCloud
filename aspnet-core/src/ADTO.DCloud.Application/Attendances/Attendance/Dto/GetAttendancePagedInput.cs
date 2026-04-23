using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.Timing;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    public class GetAttendancePagedInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 考勤状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 是否导出excel
        /// </summary>
        public bool IsExcel { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " AttDate desc";
            }
            Keyword = Keyword?.Trim();
        }

    }
}
