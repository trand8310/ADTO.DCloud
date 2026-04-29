using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.Surveys.Dto
{
    /// <summary>
    /// 考卷Dto
    /// </summary>
    [AutoMap(typeof(Survey))]
    public class SurveyDto: FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 试卷名称
        /// </summary>
        [StringLength(225)]
        public string Name { get; set; }
        /// <summary>
        /// 考试开始时间
        /// </summary>
        public DateTime StarDate { get; set; }
        /// <summary>
        /// 考试结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 答题计时(按分钟计算)
        /// </summary>
        public int Timer { get; set; }

        /// <summary>
        /// 题库来源(题库分类Id,多选 按逗号分隔)
        /// 714e1e63-5f73-4c75-9b07-08db9e22cebc,5eee37c6-69e5-4906-9b08-08db9e22cebc,02854d0e-3b26-4136-88f2-b967222de344
        /// </summary>
        [StringLength(225)]
        public string QuestionSource { get; set; }

        /// <summary>
        /// 选择题分数
        /// </summary>
        public decimal ChoiceQuestionScore { get; set; }
        /// <summary>
        /// 判断题分数
        /// </summary>
        public decimal IsQuestionScore { get; set; }
        /// <summary>
        /// 填空题分数
        /// </summary>
        public decimal FillQuestionScore { get; set; }
        /// <summary>
        /// 简单题分数
        /// </summary>
        public decimal AnswerQuestionScore { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
