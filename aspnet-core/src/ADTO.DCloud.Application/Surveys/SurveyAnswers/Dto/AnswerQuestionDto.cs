
using ADTOSharp.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyAnswers.Dto
{
    /// <summary>
    /// 返回后端,答卷题库Dto
    /// </summary>
    public class AnswerQuestionDto:EntityDto<Guid>
    {
        /// <summary>
        /// 题库名称 
        /// </summary>
        [StringLength(225)]
        public string QuestionName { get; set; }
        /// <summary>
        ///  题型(1=单选,2=多选3=判断,4=填空,4=问答)
        /// </summary>
        [StringLength(100)]
        public string QuestionType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        /// 答题所得分数
        /// </summary>
        public decimal AnswerScore { get; set; }
        /// <summary>
        /// 答案，用户输入答案
        /// </summary>
        public  string Answer { get; set; }
        /// <summary>
        /// 正确答案
        /// </summary>
        public string CorrectAnswer { get; set; }
        /// <summary>
        ///  题库选项，json格式{"OptionPrefix":"选项前缀(例如A.B.C.D.)","OptionText":"选项内容"}
        /// </summary>
        public string Option { get; set; }
    }
}
