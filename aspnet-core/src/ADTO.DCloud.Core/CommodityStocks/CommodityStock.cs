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
    /// 商品库存
    /// </summary>
    [Table("CommodityStocks")]
    public class CommodityStock : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 库存名称
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        /// <summary>
        /// 库存类别
        /// </summary>
        public CommodityStockCategory Category { get; set; }

        public virtual Guid CategoryId { get; set; }
        /// <summary>
        /// 商品型号
        /// </summary>
        [StringLength(128)]
        public string Model { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        [StringLength(128)]
        public string SN { get; set; }
        /// <summary>
        /// 库存价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 库存地址
        /// </summary>
        [StringLength(150)]
        public string Address { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// 库存状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(225)]
        public string Description { get; set; }
    }
}
