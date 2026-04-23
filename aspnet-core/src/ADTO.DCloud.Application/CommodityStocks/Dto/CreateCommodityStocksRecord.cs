
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 新增库存操作记录Dto
    /// </summary>
    [AutoMap(typeof(CommodityStocksRecord))]
    public class CreateCommodityStocksRecord
    {
        /// <summary>
        /// 库存Id
        /// </summary>
        public Guid CommodityStockId { get; set; }
        /// <summary>
        /// 库存状态
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
