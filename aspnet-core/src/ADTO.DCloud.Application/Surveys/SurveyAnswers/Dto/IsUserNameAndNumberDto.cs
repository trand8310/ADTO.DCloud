using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    /// <summary>
    /// 判断答卷人的工号姓名是否正确
    /// </summary>
    public class IsUserNameAndNumberDto
    {
        /// <summary>
        /// 答卷人名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 答卷人工号
        /// </summary>
        public string UserName { get; set; }
    }
}
