
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存统计请求参数实体
    /// </summary>
    public class PagedCommodityStockStatisticsRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键字（库存类别名称）
        /// </summary>
        public string Keyword { get; set; }
    }
}
