
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Timing;
using System;
using System.Text.Json.Serialization;


namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 4P测试题库信息
    /// </summary>
    public class PagedOA4PQuestionResultRequestDto : PagedResultRequestDto
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
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } 

    }
}