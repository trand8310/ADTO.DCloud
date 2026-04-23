using ADTOSharp.Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Surveys
{
    /// <summary>
    /// 问卷明细
    /// </summary>
    [Table("SurveyAnswerDetails")]
    public class SurveyAnswerDetail : Entity<Guid>
    {
        /// <summary>
        /// 答卷Id
        /// </summary>
        public Guid AnswerID { get; set; }

        /// <summary>
        /// 试卷内容
        /// </summary>
        public string AnswerContent { get; set; }

        /// <summary>
        /// 答卷内容
        /// </summary>
        public string AnswerValue { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

    }
}
