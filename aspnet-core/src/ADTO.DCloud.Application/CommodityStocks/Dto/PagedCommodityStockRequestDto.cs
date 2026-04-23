
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 分页查询
    /// </summary>
    public class PagedCommodityStockRequestDto : PagedResultRequestDto
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
        /// 库存类别
        /// </summary>
        public int Type { get; set; }
    }
}
