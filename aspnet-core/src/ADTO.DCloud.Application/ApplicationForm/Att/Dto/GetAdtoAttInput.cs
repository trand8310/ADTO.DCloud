using ADTO.DCloud.Infrastructure;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Att.Dto
{
    public class GetAdtoAttInput
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? EndDate { get; set; }
    }
}
