using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 新增电脑归还
    /// </summary>
    public class SetStockReturnDto
    {
        /// <summary>
        /// 明细记录Id
        /// </summary>
        public List<Guid> DetailIds { get; set; }

        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime? BackTime { get; set; }

        /// <summary>
        /// 归还用户
        /// </summary>
        public string BackUser { get; set; }

        /// <summary>
        /// 归还用户
        /// </summary>
        public string BackUserId { get; set; }

        /// <summary>
        /// 归还备注
        /// </summary>
        public string BackRemark { get; set; }
    }
}
