using System;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.CodeTable.Dto
{
    /// <summary>
    /// 数据库表字段信息
    /// </summary>
    [AutoMap(typeof(CodeColumns))]
    public class CodeColumnsDto : EntityDto<Guid>
    {
        /// <summary>
        /// 表外键【lr_db_codetable】
        /// </summary>
        public Guid CodeTableId { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string DbColumnName { get; set; }

        /// <summary>
        /// 是否自增【1是，0否】
        /// </summary>
        public int? IsIdentity { get; set; }

        /// <summary>
        /// 是否主键【1是，0否】
        /// </summary>
        public int? IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否为空【1是，0否】
        /// </summary>
        public int? IsNullable { get; set; }

        /// <summary>
        /// C#字段类型
        /// </summary>
        public string CsType { get; set; }

        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int? Length { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int? DecimalDigits { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
