
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
    /// 商品库存
    /// </summary>
    [AutoMap(typeof(CommodityStock))]
    public class CommodityStockDto : FullAuditedEntityDto<Guid>
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
        /// <summary>
        /// 类别Id
        /// </summary>
        public Guid CategoryId { get; set; }
        public string CategoryText { get; set; }
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
        public int Number { get; set; }
        /// <summary>
        /// 库存总数量=库存数量+库存变更记录新增数量
        /// </summary>
        public int TotalNumber { get; set; }
        /// <summary>
        /// 库存状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        [StringLength(225)]
        public string Description { get; set; }

        /// <summary>
        /// 地址详情
        /// </summary>
        public string AddressDetail { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 领用数量
        /// </summary>
        public string StockApplyNum { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public string RemainingNum { get; set; }
        /// <summary>
        /// 关联表
        /// </summary>
        public string Table { get; set; }
    }
}
