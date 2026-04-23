using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances
{
    /// <summary>
    /// 办公地点信息表
    /// </summary>
    [Description("办公地点信息表"), Table("AttendanceLocations")]
    public class AttendanceLocation : FullAuditedEntity<Guid>, IPassivable, IRemark, IDisplayOrder
    {
        /// <summary>
        /// 位置名称
        /// </summary>
        [StringLength(50)]
        public string LocationName { get; set; }
        /// <summary>
        /// 办公地点经纬度
        /// </summary>
        [StringLength(50)]
        public string LocationLatOrLon { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [StringLength(255)]
        public string Address { get; set; }

        /// <summary>
        /// 考勤范围(单位米)
        /// </summary>
        public long? AttendanceRadius { get; set; }

        // -- 餐补相关字段（新增）
        /// <summary>
        /// 是否餐补
        /// </summary>
        public bool HasMealAllowance { get; set; }
        /// <summary>
        /// 餐补类型：Lunch(午餐), Dinner(晚餐), Both(午晚餐)
        /// </summary>
        [StringLength(50)]
        public string MealAllowanceType { get; set; }
        /// <summary>
        /// 餐补金额
        /// </summary>
        public double? MealAllowanceAmount { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
