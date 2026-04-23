
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    /// <summary>
    /// 前端提交试卷Dto
    /// </summary>
    public class SubmitSurveyAnswerDto
    {
        /// <summary>
        /// 试卷编号
        /// </summary>
        public Guid SurveyId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public  string UserName  { get; set; }

        /// <summary>
        /// 题库信息
        /// </summary>
        public string AnswerContent { get; set; }

        /// <summary>
        /// 答卷信息
        /// </summary>
        public string AnswerValue { get; set; }
        /// <summary>
        /// 已答题数
        /// </summary>
        public int AnswerQuestionAmount { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime StarDate { get; set; }
    }
}
