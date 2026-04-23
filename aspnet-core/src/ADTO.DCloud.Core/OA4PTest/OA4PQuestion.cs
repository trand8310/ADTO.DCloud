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
    /// 4p测试题库信息
    /// </summary>
    [Table("OA4PQuestion")]
    public class OA4PQuestion : FullAuditedEntity<Guid>
    {  /// <summary>
       /// 标题
       /// </summary>
        [StringLength(128)]
        public string QuestionsTitle { get; set; }
        /// <summary>
        /// 有效标志0否1是
        /// </summary>
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// 选项一
        /// </summary>
        [StringLength(500)]
        public string Option_1 { get; set; }
        /// <summary>
        /// 选项二
        /// </summary>
        [StringLength(500)]
        public string Option_2 { get; set; }
        /// <summary>
        /// 选项三
        /// </summary>
        [StringLength(500)]
        public string Option_3 { get; set; }
        /// <summary>
        /// 选项四
        /// </summary>
        [StringLength(500)]
        public string Option_4 { get; set; }
        /// <summary>
        /// 题库类型
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Type { get; set; }
    }
}
