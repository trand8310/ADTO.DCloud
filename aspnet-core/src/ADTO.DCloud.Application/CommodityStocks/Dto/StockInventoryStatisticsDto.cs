using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 电脑归还-导出统计
    /// </summary>
    public class StockInventoryStatisticsDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 已领用设备
        /// </summary>
        public string ProductNames { get; set; }
    }
}
