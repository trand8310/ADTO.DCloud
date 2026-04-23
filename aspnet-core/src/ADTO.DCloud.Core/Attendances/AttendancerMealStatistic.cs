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
    /// 考勤餐补
    /// </summary>
    [Table("AttendancerMealStatistics")]
    public class AttendancerMealStatistic:Entity<Guid>
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// 餐补月份
        /// </summary>
        public DateTime AttDate { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [StringLength(128)]
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// 中餐餐补次数
        /// </summary>
        public int LunchCount { get; set; }
        /// <summary>
        /// 中餐餐补费用（15元/餐）
        /// </summary>
        public decimal LunchPrice { get; set; }
        /// <summary>
        /// 晚餐餐补次数
        /// </summary>
        public int DinnerCount { get; set; }
        /// <summary>
        /// 晚餐餐补费用（15元/餐）
        /// </summary>
        public decimal DinnerPrice { get; set; }

        /// <summary>
        /// 合计
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remarks { get; set; }
    }
}
