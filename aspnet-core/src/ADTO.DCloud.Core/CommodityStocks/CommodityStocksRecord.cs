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
    /// 库存操作记录、变更记录
    /// </summary>
    [Table("CommodityStocksRecords")]
    public class CommodityStocksRecord : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 库存
        /// </summary>
        public CommodityStock CommodityStock { get; set; }
        public virtual Guid CommodityStockId { get; set; }
        /// <summary>
        /// 库存状态 字典
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 价值
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(225)]
        public string Description { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperationDate { get; set; }
    }
}
