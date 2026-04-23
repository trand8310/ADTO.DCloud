using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Surveys
{
    /// <summary>
    /// 题库类别
    /// </summary>
    [Table("SurveyQuestionCategory")]
    public class SurveyQuestionCategory : FullAuditedEntity<Guid>, IMayHaveTenant, IRemark, IPassivable, IDisplayOrder
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        [StringLength(225)]
        public string Name { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否有效、启用
        /// </summary>
        [Description("是否有效、启用")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
