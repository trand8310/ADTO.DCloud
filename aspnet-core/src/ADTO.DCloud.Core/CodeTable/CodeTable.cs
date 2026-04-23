using System;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ADTO.DCloud.CodeTable
{
    /// <summary>
    /// (数据库表信息)表的实体
    /// </summary>
    [Description("数据库表信息"), Table("CodeTables")]

    public class CodeTable : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IRemark
    {
        /// <summary>
        /// 类名
        /// </summary>
        [Description("类名")]
        public string ClassName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string TableName { get; set; }

        /// <summary>
        /// 数据库连接外键【lr_base_databaselink】
        /// </summary>
        [Description("数据库连接外键【lr_base_databaselink】")]
        public string DbId { get; set; }

        /// <summary>
        /// 是否锁表【1是，0否】
        /// </summary>
        [Description("是否锁表【1是，0否】")]
        public int? IsLock { get; set; }

        /// <summary>
        /// 模型与数据库间的同步关系 1 同步 0 未同步
        /// </summary>
        [Description("模型与数据库间的同步关系 1 同步 0 未同步")]
        public int? State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }
    }
}
