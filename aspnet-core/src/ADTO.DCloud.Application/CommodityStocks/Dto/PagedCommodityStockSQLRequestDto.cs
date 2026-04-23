
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    /// <summary>
    /// 电脑库存-sql查询参数
    /// </summary>
    public class PagedCommodityStockSQLRequestDto : PagedAndSortedResultRequestDto
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

        /// <summary>
        /// 产品状态
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 产品型号
        /// </summary>

        public string Model { get; set; }

        /// <summary>
        /// SN
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyUser { get; set; }
        /// <summary>
        /// 库存位置
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 查询剩余库存数量
        /// </summary>
        public int IsRemainingNum { get; set; }
    }
}
