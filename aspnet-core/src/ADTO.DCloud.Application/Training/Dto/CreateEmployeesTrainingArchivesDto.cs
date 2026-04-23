

using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Timing;
using System;
using System.Text.Json.Serialization;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 新增培训记录
    /// </summary>
    [AutoMapTo(typeof(EmployeesTrainingArchives))]
    public class CreateEmployeesTrainingArchivesDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 用户工号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 培训日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime TrainingDate { get; set; }

        /// <summary>
        /// 培训标题
        /// </summary>
        public string TrainingTitle { get; set; }

        /// <summary>
        /// 培训课时
        /// </summary>
        public decimal TrainingHour { get; set; }

        /// <summary>
        /// 培训级别
        /// </summary>
        public string TrainingLevel { get; set; }

        /// <summary>
        /// 培训分数
        /// </summary>
        public string TrainingScore { get; set; }

    }
}
