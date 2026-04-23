
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Timing;
using System;
using System.Text.Json.Serialization;


namespace ADTO.DCloud.EmployeeManager.Dto
{
    public class Paged4PStatisticResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }

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
