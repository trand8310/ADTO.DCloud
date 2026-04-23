using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
 
namespace ADTO.DCloud.Surveys.QuestionCategorys.Dto
{
    [AutoMap(typeof(SurveyQuestionCategory))]
    public class QuestionCategoryDto:  FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        [StringLength(225)]
        public string Name { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否有效、启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
