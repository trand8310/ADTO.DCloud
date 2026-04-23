using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    public class DeleteByUserIdSurveyIdDto
    {
        /// <summary>
        /// 考卷Id
        /// </summary>
        public Guid? SurveyId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }

    }
}
