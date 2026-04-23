using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks
{
    /// <summary>
    /// 商品库存类别
    /// </summary>
    [Table("CommodityStockCategory")]
    public class CommodityStockCategory : FullAuditedEntity<Guid>
    {
        /// <summary>
        ///父级类别
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [StringLength(128)]
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 库存类别用来区分是电脑库存还是总裁办库存-枚举（EnumCommodityStockType）
        /// </summary>
        [StringLength(128)]
        public int Type { get; set; }
    }
}
