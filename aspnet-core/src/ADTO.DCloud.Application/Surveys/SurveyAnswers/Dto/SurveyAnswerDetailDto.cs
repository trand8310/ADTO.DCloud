
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
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
    [AutoMap(typeof(SurveyAnswerDetail))]
    public class SurveyAnswerDetailDto : Entity<Guid>
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
    }
}
