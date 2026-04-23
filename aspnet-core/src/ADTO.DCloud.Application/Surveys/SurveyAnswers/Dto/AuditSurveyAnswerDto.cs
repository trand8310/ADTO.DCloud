
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditSurveyAnswerDto : EntityDto<Guid>
    {

        /// <summary>
        /// 分数
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        /// 答卷信息
        /// </summary>
        public string AnswerValue { get; set; }
        /// <summary>
        /// 题库信息
        /// </summary>
        public string AnswerContent { get; set; }
    }
}
