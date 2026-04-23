using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存记录
    /// </summary>
    public class PagedCommodityStockRecordRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 库存Id
        /// </summary>
        public Guid? CommodityStockId { get; set; }
        /// <summary>
        /// 操作状态
        /// </summary>
        public int? Status { get; set; }
    }
}
