using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    public class AttendanceCheckTimeDto
    {
        public DateTime CheckTime { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SENSORID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SN { get; set; }
    }
}
