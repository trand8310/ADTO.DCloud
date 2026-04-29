using ADTO.DCloud.Infrastructure;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Attendances.DingDingAttLogs.Dto
{
    public class GetDingdingUserAttLogInput
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// 结束日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1).AddTicks(-1);
        /// <summary>
        /// 用户工号
        /// </summary>
        public string UserName { get; set; }
    }
}
