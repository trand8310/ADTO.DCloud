using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    /// <summary>
    /// 考勤机记录和钉钉打卡记录Dto
    /// </summary>
    public class AttendanceRecordOrDingdingDto
    {
        /// <summary>
        /// UserId
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// EnrollNumber
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// CompanyName
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public Guid? DeptpartmentId { get; set; }
        /// <summary>
        /// DeptName
        /// </summary>
        public string DeptpartmentName { get; set; }
        /// <summary>
        /// AttDate
        /// </summary>
        public DateTime AttDate { get; set; }
        /// <summary>
        /// 考勤机号
        /// </summary>
        public string SENSORID { get; set; }
        /// <summary>
        /// 类型-钉钉或考勤机
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// MachineAddress
        /// </summary>
        public string MachineAddress { get; set; }
    }
}
