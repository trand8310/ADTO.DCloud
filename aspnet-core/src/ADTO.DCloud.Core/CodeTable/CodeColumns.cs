using System;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ADTO.DCloud.CodeTable
{
    /// <summary>
    /// 数据库表字段信息
    /// </summary>
    [Description("数据库表字段信息"), Table("CodeColumns")]
    public class CodeColumns : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IRemark, IDisplayOrder
    {
        /// <summary>
        /// 表外键【lr_db_codetable】
        /// </summary>
        [ForeignKey("CodeTable")]
        [Description("表外键Id")]
        public Guid CodeTableId { get; set; }
        public virtual CodeTable CodeTable { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        [Description("列名")]
        public string DbColumnName { get; set; }

        /// <summary>
        /// 是否自增【1是，0否】
        /// </summary>
        [Description("是否自增【1是，0否】")]
        public int? IsIdentity { get; set; }

        /// <summary>
        /// 是否主键【1是，0否】
        /// </summary>
        [Description("是否主键【1是，0否】")]
        public int? IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否为空【1是，0否】
        /// </summary>
        [Description("是否为空【1是，0否】")]
        public int? IsNullable { get; set; }

        /// <summary>
        /// C#字段类型
        /// </summary>
        //[Description("C#字段类型")]
        public string CsType { get; set; }

        /// <summary>
        /// 数据库字段类型
        /// </summary>
        //[Description("数据库字段类型")]
        public string DbType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        [Description("字段长度")]
        public int? Length { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        [Description("小数位数")]
        public int? DecimalDigits { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Description("表外键租户IDId")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        [Description("排序值")]
        public int DisplayOrder { get; set; }
    }
}
