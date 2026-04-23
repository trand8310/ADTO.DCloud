

using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Timing;
using System;
using System.Text.Json.Serialization;

namespace ADTO.DCloud.Training.Dto
{
    /// <summary>
    /// 员工培训记录
    /// </summary>
    [AutoMap(typeof(EmployeesTrainingArchives))]
    public class EmployeesTrainingArchivesDto : FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户工号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 职级
        /// </summary>
        public string PostLevelId { get; set; }

        /// <summary>
        /// 职级
        /// </summary>
        public string PostLevelText { get; set; }

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

        /// <summary>
        /// 培训月份
        /// </summary>
        public int TrainingMonth { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorUserName { get; set; }
    }
}
