using ADTO.DCloud.CommodityStocks.Dto;
using System;
using System.Collections.Generic;

namespace ADTO.DCloud.CommodityStocks
{
    /// <summary>
    /// 库存类别
    /// </summary>
    public interface ICommodityStockCategoryAppService
    {
        /// <summary>
        /// 得到库存类别列表，非分页
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<CommodityStockCategoryDto> GetStockCategoryList(PagedCommodityStockCategoryRequestDto input);

    }
}
