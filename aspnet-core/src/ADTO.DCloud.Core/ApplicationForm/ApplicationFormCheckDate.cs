using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm
{
    /// <summary>
    /// 表单申请-特殊情况允许申请(请假、外出、出差、考勤异常表)
    /// </summary>
    [Table("ApplicationFormCheckDates")]
    public class ApplicationFormCheckDate : Entity<int>
    {
        /// <summary>
        /// 用户工号
        /// </summary>
        [StringLength(120)]
        public string UserName { get; set; }

        /// <summary>
        /// 申请结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsAudit { get; set; }
        /// <summary>
        /// 申请类型（出差、外出、考勤异常、请假）
        /// </summary>
        [StringLength(120)]
        public string Type { get; set; }

    }
}
