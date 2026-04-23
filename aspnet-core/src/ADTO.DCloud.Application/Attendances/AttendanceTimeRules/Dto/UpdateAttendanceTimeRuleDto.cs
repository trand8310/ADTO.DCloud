using ADTO.DCloud.Attendances;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.AttendanceTimeRules.Dto
{
    [AutoMap(typeof(AttendanceTimeRule))]
    public class UpdateAttendanceTimeRuleDto : EntityDto<Guid>
    {
        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// 规则备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 规则排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 考勤时间集合
        /// </summary>
        public string AttendanceTimeIds { get; set; }
        /// <summary>
        /// 考勤地点
        /// </summary>
        public virtual Guid? LocationId { get; set; }
    }
}
