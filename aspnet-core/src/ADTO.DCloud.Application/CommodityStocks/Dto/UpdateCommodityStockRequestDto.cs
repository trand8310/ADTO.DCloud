
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存数量修改Dto
    /// </summary>
    public class UpdateCommodityStockRequestDto:EntityDto<Guid>
    {
        /// <summary>
        /// 库存数量
        /// </summary>
        public int Number { get; set; }
    }
}
