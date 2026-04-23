using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.DatabaseManager
{
    /// <summary>
    /// 数据表管理（导入表源表）
    /// </summary>
    [Description("导入表源表"), Table("ImportTableSources")]
    public class ImportTableSource : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表说明描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Description("表外键租户IDId")]
        public Guid? TenantId { get; set; }
    }
}
