using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances
{
    /// <summary>
    /// 考勤时间表
    /// </summary>
    [Description("考勤时间表"), Table("AttendanceTimes")]
    public class AttendanceTime : FullAuditedEntity<Guid>, IPassivable, IRemark
    {
        [StringLength(128)]
        public string Name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime SDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EDate { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        /// <returns></returns>
        [Required]
        public TimeSpan AMInTime { get; set; }
        /// <summary>
        /// 上午下班时间
        /// </summary>
        /// <returns></returns>
        [Required]
        public TimeSpan AMOutTime { get; set; }
        /// <summary>
        /// 下午上班时间
        /// </summary>
        /// <returns></returns>
        [Required]
        public TimeSpan PMInTime { get; set; }
        /// <summary>
        /// 下午下班时间
        /// </summary>
        /// <returns></returns>
        [Required]
        public TimeSpan PMOutTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }
    }
}
