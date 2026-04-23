using System;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.CommodityStocks.PresidentofficeStock.Dto
{
    /// <summary>
    /// 总裁办库存管理分页查询条件
    /// </summary>
    public class PagedPresidentofficeStockRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 关键字（产品名称）
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 库存类别
        /// </summary>
        public Guid? CategoryId { get; set; }
      
        /// <summary>
        /// 产品型号、规格
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyUser { get; set; }
       
    }
}
