using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataReports.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class MobileHomeStatRequestDto
    {
        /// <summary>
        /// 日期类型
        /// </summary>
        [Required(ErrorMessage ="日期类型不能为空")]
        public string DateType { get; set; }
    }
}

public enum StatTimeRange
{
    Today,              //今天
    CurrentMonth,       // 本月
   // PreviousMonth,      // 上月
    CurrentQuarter,     // 本季度
    CurrentYear,        // 本年
 //   PreviousYear,       // 上年度
   // Custom             // 自定义范围
}
