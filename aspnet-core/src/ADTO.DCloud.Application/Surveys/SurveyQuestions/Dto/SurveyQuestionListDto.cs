using ADTO.DCloud.Infrastructure;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyQuestions.Dto
{
    public class SurveyQuestionListDto
    {
        /// <summary>
        /// 试卷名称
        /// </summary>
        [StringLength(225)]
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 答题时间
        /// </summary>
        public int Timer { get; set; }
        /// <summary>
        /// 答题时间
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime StarDate { get; set; }=DateTime.Now;
        /// <summary>
        /// 考卷题库信息
        /// </summary>
        public List<SurveyQuestionPCDto> SurveyQuestions { get; set; }
    }
}
