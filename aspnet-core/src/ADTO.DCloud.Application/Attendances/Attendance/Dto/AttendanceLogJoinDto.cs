using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    public class AttendanceLogJoinDto : EntityDto<Guid>
    {

        /// <summary>
        /// 用ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// EnrollNumber
        /// </summary>
        /// <returns></returns>
        public string UserName { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }
        /// <summary>
        /// CompanyId
        /// </summary>
        /// <returns></returns>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// DeptID
        /// </summary>
        /// <returns></returns>
        public Guid? DepartmentId { get; set; }
        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime AttDate { get; set; }
        /// <summary>
        /// 上午上班时间
        /// </summary>
        public TimeSpan AMInTime { get; set; }
        /// <summary>
        /// 上午下班时间
        /// </summary>
        public TimeSpan AMOutTime { get; set; }
        /// <summary>
        /// 上午签到时间
        /// </summary>
        public TimeSpan AMInAttTime { get; set; }
        /// <summary>
        /// 上午下班时间
        /// </summary>
        public TimeSpan AMOutAttTime { get; set; }
        /// <summary>
        /// 上午上班签到区域-考勤地点
        /// </summary>
        public Guid? AMInLocationId { get; set; }
        /// <summary>
        /// 上午下班签到区域-考勤地点
        /// </summary>
        public Guid? AMOutLocationId { get; set; }
        /// <summary>
        /// 下午上班时间
        /// </summary>
        public TimeSpan PMInTime { get; set; }
        /// <summary>
        /// 下午下班时间
        /// </summary>
        public TimeSpan PMOutTime { get; set; }
        /// <summary>
        /// 下午上班时间
        /// </summary>
        public TimeSpan PMInAttTime { get; set; }
        /// <summary>
        /// 下午下班时间
        /// </summary>
        public TimeSpan PMOutAttTime { get; set; }
        /// <summary>
        /// 下午上班区域-考勤地点
        /// </summary>
        public Guid? PMInLocationId { get; set; }
        /// <summary>
        /// 下午下班区域-考勤地点
        /// </summary>
        public Guid? PMOutLocationId { get; set; }
        /// <summary>
        /// 上午上班状态
        /// </summary>
        [StringLength(128)]
        public string AMInType { get; set; }
        /// <summary>
        /// 上午下班状态
        /// </summary>
        [StringLength(128)]
        public string AMOutType { get; set; }
        /// <summary>
        /// 下午上班状态
        /// </summary>
        [StringLength(128)]
        public string PMInType { get; set; }
        /// <summary>
        /// 下午下班状态
        /// </summary>
        [StringLength(128)]
        public string PMOutType { get; set; }

        /// <summary>
        /// 默认考勤区域-考勤地点
        /// </summary>
        public Guid? LocationId { get; set; }
        #region 扩展字段
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartName { get; set; }
        #endregion

        /// <summary>
        /// 常驻办公点
        /// </summary>
        [NotMapped]
        public Guid? OfficeLocation { get; set; }
        /// <summary>
        /// 打卡类型（枚举：EnumUserCheckInType）
        /// </summary>
        [NotMapped]
        public int CheckInType { get; set; }
        /// <summary>
        /// 是否周六考勤-1：是0：否
        /// </summary>
        public int IsAttDateDayOfWeek { get; set; }

        /// <summary>
        /// 上午上班-办公地点
        /// </summary>
        public string AMInLocation { get; set; }
        /// <summary>
        /// 上午下班签到区域-考勤地点-办公地点
        /// </summary>
        public string AMOutLocation { get; set; }
        /// <summary>
        /// 下午上班区域-考勤地点-办公地点
        /// </summary>
        public string PMInLocation { get; set; }
        /// <summary>
        /// 下午下班区域-考勤地点-办公地点
        /// </summary>
        public string PMOutLocation { get; set; }
    }
}
