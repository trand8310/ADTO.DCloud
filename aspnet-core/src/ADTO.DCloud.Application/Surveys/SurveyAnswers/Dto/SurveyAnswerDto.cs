
using ADTOSharp.Application.Services.Dto;
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
    /// 答卷
    /// </summary>
    [AutoMap(typeof(SurveyAnswer))]
    public class SurveyAnswerDto : FullAuditedEntityDto<Guid>
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
        /// 答卷人名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 答卷人工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 答卷状态(1=尚未收到问卷,2=已收到问卷,3=已开始答卷,4=已交卷,5=已收到回执)
        /// </summary>
        public int AnswerStatus { get; set; }
        /// <summary>
        /// 开始答卷时间
        /// </summary>
        public DateTime? AnswerStartTime { get; set; }
        /// <summary>
        /// 结束答卷时间(交卷时间)
        /// </summary>
        public DateTime? AnswerEndTime { get; set; }
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
