using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Surveys
{
    /// <summary>
    /// 题库
    /// </summary>
    [Table("SurveyQuestion")]
    public class SurveyQuestion : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 题库名称
        /// </summary>
        [StringLength(225)]
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
        [StringLength(100)]
        public string QuestionType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }

        /// <summary>
        /// 正确答案
        /// </summary>
        [StringLength(225)]
        public string CorrectAnswer { get; set; }

        /// <summary>
        ///  题库选项，json格式{"OptionPrefix":"选项前缀(例如A.B.C.D.)","OptionText":"选项内容"}
        /// </summary>
        public string Option { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }
    }
}
