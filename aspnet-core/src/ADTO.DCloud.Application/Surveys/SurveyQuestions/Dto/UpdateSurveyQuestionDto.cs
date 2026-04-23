
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.SurveyQuestions.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(SurveyQuestion))]
    public class UpdateSurveyQuestionDto : EntityDto<Guid>
    {
        /// <summary>
        /// 题库名称
        /// </summary>
        public string QuestionName { get; set; }
        /// <summary>
        /// 题库类别编号
        /// </summary>
        public Guid QuestionCategoryId { get; set; }
        /// <summary>
        /// 考卷编号
        /// </summary>
        public Guid? SurveyID { get; set; }
        /// <summary>
        ///  题型(1=单选,2=多选,3=填空,4=问答)
        /// </summary>
        public string QuestionType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }
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
