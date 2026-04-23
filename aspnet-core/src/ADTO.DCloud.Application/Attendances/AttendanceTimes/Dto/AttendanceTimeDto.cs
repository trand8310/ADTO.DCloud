using ADTO.DCloud.Attendances;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceTimes.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(AttendanceTime))]
    public class AttendanceTimeDto:FullAuditedEntityDto<Guid>
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
