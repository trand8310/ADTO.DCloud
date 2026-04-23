using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.PresidentofficeStock.Dto
{
    /// <summary>
    ///总裁办物品申请管理查询 条件
    /// </summary>
    public class GetOfficeSupplyApplicationPagedRequest : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 申请产品ID
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 物品费用状态
        /// </summary>
        public int? ApplyType { get; set; }

        /// <summary>
        /// 物品类别
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// 申请人（工号、名称）
        /// </summary>
        public string ApplyUser { get; set; }

        /// <summary>
        /// 申请开始时间
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 申请结束时间
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? EndTime { get; set; }
    }
}
