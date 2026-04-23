using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.OA4PTest
{
    /// <summary>
    /// 4P测试
    /// </summary>
    [Table("OA4PStatistics")]
    public class OA4PStatistic : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 真实姓名
        /// </summary>
        [Required]
        [StringLength(128)]
        public string TrueName { get; set; }
        /// <summary>
        /// 应聘职位
        /// </summary>
        [Required]
        [StringLength(128)]
        public string PostName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Mobile { get; set; }
        /// <summary>
        /// 选项一选择个数
        /// </summary>
        public int? Option_1_Total { get; set; }
        /// <summary>
        /// 选项二选择个数
        /// </summary>
        public int? Option_2_Total { get; set; }
        /// <summary>
        /// 选项三选择个数
        /// </summary>
        public int? Option_3_Total { get; set; }
        /// <summary>
        /// 选项四选择个数
        /// </summary>
        public int? Option_4_Total { get; set; }
        /// <summary>
        /// 总答题个数
        /// </summary>
        public int? Option_Sum_Total { get; set; }
        /// <summary>
        /// 用时（分钟）
        /// </summary>
        public string UesTime { get; set; }
        /// <summary>
        /// 答题数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 答卷类型
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Type { get; set; }

        /// <summary>
        /// 邀请人
        /// </summary>
        [StringLength(128)]
        public string Inviter { get; set; }

    }
}
