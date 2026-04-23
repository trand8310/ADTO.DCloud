using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.CodeRule
{
    /// <summary>
    /// 编码生成记录表
    /// </summary>
    [Description("编码生成记录表"), Table("CodeRuleRecords")]
    public class CodeRuleRecord:Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        /// <summary>
        ///  编码规则主键Id
        /// </summary>
        [ForeignKey("CodeRule")]
        [Description("编码规则主键Id")]
        public Guid RuleId { get; set; }
        public virtual CodeRule Rule { get; set; }

        /// <summary>
        /// 生成的编码
        /// </summary>
        public string GeneratedCode { get; set; }

        /// <summary>
        /// 业务记录Id
        /// </summary>
        public Guid? BusinessId { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Description("创建人")]
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreationTime { get; set; }
    }
}
