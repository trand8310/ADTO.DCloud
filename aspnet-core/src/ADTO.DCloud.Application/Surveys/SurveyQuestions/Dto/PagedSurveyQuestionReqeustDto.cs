using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyQuestions.Dto
{
    public class PagedSurveyQuestionReqeustDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 题库类型
        /// </summary>
        public string QuestionType { get; set; }
        /// <summary>
        /// 题库类别
        /// </summary>
        public Guid? CategoryId { get; set; }
    }
}
