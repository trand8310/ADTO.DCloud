using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    public class GetRequestTreeInput
    {
        public int Type { get; set; } = (int)EnumCommodityStockType.CommodityStocks;//默认查询电脑库存
    }
}
