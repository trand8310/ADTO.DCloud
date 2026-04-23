
using ADTO.DCloud.OA4PTest;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 4ptest
    /// </summary>
    [AutoMap(typeof(OA4PQuestion))]
    public class OA4PQuestionDto : EntityDto<Guid>
    {
        /// <summary>
        /// 题库类型
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Type { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        /// </summary>
        [Required(ErrorMessage = "题目不能为空")]
        [StringLength(128)]
        public string QuestionsTitle { get; set; }
        /// <summary>
        /// 有效标志0否1是
        /// </summary>
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
        /// <summary>
        /// 选项一
        /// </summary>
        [StringLength(500)]
        public string Option_1 { get; set; }
        /// <summary>
        /// 选项二
        /// </summary>
        [StringLength(500)]
        public string Option_2 { get; set; }
        /// <summary>
        /// 选项三
        /// </summary>
        [StringLength(500)]
        public string Option_3 { get; set; }
        /// <summary>
        /// 选项四
        /// </summary>
        [StringLength(500)]
        public string Option_4 { get; set; }
        /// <summary>
        /// 开始答题时间
        /// </summary>
        public DateTime StarTime { get; set; }= DateTime.Now;

    }
}
