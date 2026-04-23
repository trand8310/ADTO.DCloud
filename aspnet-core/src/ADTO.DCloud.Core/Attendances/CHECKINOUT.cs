using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances
{
    /// <summary>
    /// 考勤机打卡记录
    /// </summary>

    [Table("temp_CHECKINOUT")]
    public class CHECKINOUT : Entity<int>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int USERID { get; set; }
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

        /// <summary>
        /// 
        /// </summary>
        public string Memoinfo { get; set; }

        public string WorkCode { get; set; }

        public short UserExtFmt { get; set; }

        public int mask_flag { get; set; }

        public float temperature { get; set; }

    }
}
