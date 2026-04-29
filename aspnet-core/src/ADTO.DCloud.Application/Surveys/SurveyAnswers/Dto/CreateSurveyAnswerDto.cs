
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    /// <summary>
    /// 新增Dto
    /// </summary>
    [AutoMap(typeof(SurveyAnswer))]
    public class CreateSurveyAnswerDto 
    {
        /// <summary>
        /// 考卷编号
        /// </summary>
        public Guid SurveyID { get; set; }
        /// <summary>
        /// 答卷提交人
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 答卷状态(0=未答卷,2=已交卷,3=已阅卷)
        /// </summary>
        public int AnswerStatus { get; set; }
        /// <summary>
        /// 开始答卷时间
        /// </summary>
        public DateTime AnswerStartTime { get; set; }
        /// <summary>
        /// 结束答卷时间(交卷时间)
        /// </summary>
        public DateTime AnswerEndTime { get; set; }
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
    }
}
