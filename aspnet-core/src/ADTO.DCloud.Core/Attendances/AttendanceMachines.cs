using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances
{
    /// <summary>
    /// 考勤机
    /// </summary>
    [Table("AttendanceMachines")]
    public class AttendanceMachines : Entity<Guid>
    {
        /// <summary>
        /// 考勤机地址
        /// </summary>
        [StringLength(225)]
        public string MachineAddress { get; set; }

        /// <summary>
        /// 考勤机IP
        /// </summary>
        [StringLength(225)]
        public string MachineIP { get; set; }
        /// <summary>
        /// 考勤机编号
        /// </summary>
        public int MachineNumber { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// 考勤机区域-考勤地点
        /// </summary>
        public AttendanceLocation? Location { get; set; }
        /// <summary>
        /// 考勤地点
        /// </summary>
        public virtual Guid LocationId { get; set; }
    }
}
