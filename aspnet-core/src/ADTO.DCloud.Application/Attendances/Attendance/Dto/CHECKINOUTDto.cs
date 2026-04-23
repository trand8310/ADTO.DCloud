using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.Attendance.Dto
{
    public class CHECKINOUTDto : EntityDto<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 考勤时间
        /// </summary>
        public DateTime CheckTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SENSORID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 检查类型
        /// </summary>
        public string CHECKTYPE { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string VERIFYCODE { get; set; }

        #region 新增字段
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        #endregion
    }
}
