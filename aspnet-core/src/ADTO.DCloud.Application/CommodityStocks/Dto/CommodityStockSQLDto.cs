
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 电脑库存返回列表
    /// </summary>
    [AutoMap(typeof(CommodityStock))]
    public class CommodityStockSQLDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public Guid? Category { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        public string CategoryText { get; set; }
        /// <summary>
        /// 库存位置
        /// </summary>
        public string Addr { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// 领用数量
        /// </summary>
        public int? StockApplyNum { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int? RemainingNum { get; set; }
        /// <summary>
        /// 异常数量
        /// </summary>
        public int? AbnormalNum { get; set; }
        /// <summary>
        /// 库存价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 产品位置
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        public int? SortCode { get; set; }
    }
}
