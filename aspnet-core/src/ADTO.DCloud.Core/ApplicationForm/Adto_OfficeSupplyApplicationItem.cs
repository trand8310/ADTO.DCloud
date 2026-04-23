using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
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
    /// 总裁物品申请明细表
    /// </summary>
    [Table("Adto_OfficeSupplyApplicationItems")]
    public class Adto_OfficeSupplyApplicationItem : FullAuditedEntity<Guid>, IRemark
    {
        /// <summary>
        /// 总裁物品申请记录Id
        /// </summary>
        public Adto_OfficeSupplyApplication OfficeSupplyApplication { get; set; }

        public virtual Guid OfficeSupplyApplicationId { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [StringLength(100)]
        public Guid? Category { get; set; }

        /// <summary>
        /// 物品Id
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        [StringLength(100)]
        public string ProductName { get; set; }
        /// <summary>
        /// 申请类型
        /// </summary>
        public int? ApplyType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 申请数量
        /// </summary>
        public int ApplyCount { get; set; }

        /// <summary>
        /// 物品规格
        /// </summary>
        [StringLength(128)]
        public string Model { get; set; }
    }
}
