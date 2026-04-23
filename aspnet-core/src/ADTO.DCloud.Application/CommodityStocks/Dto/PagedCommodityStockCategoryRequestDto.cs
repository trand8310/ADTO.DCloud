using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存类别查询条件
    /// </summary>
    public class PagedCommodityStockCategoryRequestDto 
    {
        /// <summary>
        /// 库存类别
        /// </summary>
        public int Type { get; set; }
    }
}
