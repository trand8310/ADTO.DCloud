using ADTO.DCloud.Infrastructure;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    public class UpdateAttendanceLogsRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime StartDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime EndDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public long? DepartmentId { get; set; }
        /// <summary>
        /// 用户基础信息设置的考勤区域
        /// </summary>
        public int? AreaId { get; set; }

        /// <summary>
        /// 考勤状态Id
        /// </summary>
        public List<Guid> IdList { get; set; }
    }
}
