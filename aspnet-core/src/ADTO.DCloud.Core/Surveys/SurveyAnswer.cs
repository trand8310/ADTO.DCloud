using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
namespace ADTO.DCloud.Surveys
{
    /// <summary>
    /// 答卷
    /// </summary>
    [Table("SurveyAnswer")]
    public class SurveyAnswer : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 考卷编号
        /// </summary>
        public Guid SurveyId { get; set; }

        /// <summary>
        /// 答卷提交人
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 答卷状态(1=收到答卷,2=已交卷,3=已收到回执)
        /// </summary>
        public int AnswerStatus { get; set; }

        /// <summary>
        /// 开始答卷时间
        /// </summary>
        public DateTime? AnswerStartTime { get; set; }

        /// <summary>
        /// 结束答卷时间(交卷时间)
        /// </summary>
        public DateTime? AnswerEndTime { get; set; }

        /// <summary>
        /// 答卷花费时间
        /// </summary>
        [StringLength(128)]
        public string SpendMinutes { get; set; }

        /// <summary>
        /// 已答题目
        /// </summary>
        public int AnswerQuestionAmount { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }


    }
}
