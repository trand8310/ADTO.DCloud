using ADTO.DCloud.Attendances;
using ADTO.DCloud.EmployeeManager;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(PublicHoliDay))]
    public class PublicHoliDayDto : EntityDto<Guid>
    {
        /// <summary>
        /// HoliDay
        /// </summary>
        /// <returns></returns>
        public DateTime HoliDay { get; set; }
        /// <summary>
        /// 上班或公休
        /// </summary>
        /// <returns></returns>
        public int State { get; set; }
        /// <summary>
        /// 是否早会时间(0:否，1:是)
        /// </summary>
        /// <returns></returns>
        public int IsMorningMeetingTime { get; set; }
        /// <summary>
        /// 是否法定节假日
        /// </summary>
        /// <returns></returns>
        public int IsLegalHoliday { get; set; }
        /// <summary>
        /// 是否特殊日期
        /// </summary>
        /// <returns></returns>
        public int? IsSpecial { get; set; }

        /// <summary>
        /// 单休状态
        /// </summary>
        public int? IsSixDay { get; set; }

        /// <summary>
        /// 大小周
        /// </summary>
        public int? SizeWeek { get; set; }



    }
}
