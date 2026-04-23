using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    /// <summary>
    /// 无分页请求
    /// </summary>
    public class SurveyAnswerRequestDto
    {
        /// <summary>
        /// 考卷Id
        /// </summary>
        public Guid SurveyId { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string keyword { get; set; }
    }
}
