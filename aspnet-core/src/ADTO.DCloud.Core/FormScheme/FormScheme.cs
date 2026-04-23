using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.FormScheme
{
    /// <summary>
    /// 表单模板表
    /// 每次更新新增一条数据，一对多
    /// </summary>
    [Description("表单模板表"), Table("FormSchemes")]
    public class FormScheme: FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// 模板信息主键
        /// </summary>
        [Description("模板信息主键")]
        public Guid SchemeInfoId { get; set; }

        /// <summary>
        /// 1.正式2.草稿
        /// </summary>
        [Description("1.正式2.草稿")]
        public int? Type { get; set; }

        /// <summary>
        /// 模板
        /// </summary>
        [Description("模板")]
        public string Scheme { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]

        public Guid? TenantId { get; set; }
    }
}
