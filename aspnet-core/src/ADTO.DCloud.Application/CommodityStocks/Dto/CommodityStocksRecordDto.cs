
using ADTOSharp.Application.Services.Dto;
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
    /// 库存变更记录Dto
    /// </summary>
    [AutoMap(typeof(CommodityStocksRecord))]
    public class CommodityStocksRecordDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 库存
        /// </summary>
        public CommodityStock CommodityStock { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 库存状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 库存状态名称
        /// </summary>
        public string StatusText{ get; set; }

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
        /// 操作人
        /// </summary>
        public string CreatorUser { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperationDate { get; set; }
    }
}
