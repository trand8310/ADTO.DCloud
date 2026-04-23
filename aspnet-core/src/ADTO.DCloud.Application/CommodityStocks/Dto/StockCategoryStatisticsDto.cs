
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 库存统计返回实体
    /// </summary>
    public class StockCategoryStatisticsDto : EntityDto<Guid>
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 申请数量
        /// </summary>
        public int StockApplyNumber { get; set; }
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemainingNumber { get; set; }

        /// <summary>
        /// 异常数量
        /// </summary>
        public int AbnormalNum { get; set; }
    }
}
