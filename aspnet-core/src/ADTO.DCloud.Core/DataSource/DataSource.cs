using System;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ADTO.DCloud.DataSource
{
    /// <summary>
    /// 数据源
    /// </summary>
    [Description("数据源表"), Table("DataSources")]
    public class DataSource : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IRemark
    {
        #region 实体成员

        /// <summary>
        /// 编号
        /// </summary>
        [Description("编号")]
        public string Code { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Description("名字")]
        public string Name { get; set; }

        /// <summary>
        /// 数据库主键
        /// </summary>
        [Description("数据库主键")]
        public string DbId { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        [Description("sql语句")]
        public string Sql { get; set; }

        /// <summary>
        /// 1.表格 2.代码 3.关联 0或者其他sql语句
        /// </summary>
        [Description("1.表格 2.代码 3.关联 0或者其他sql语句")]
        public int Type { get; set; }

        /// <summary>
        /// 分类字段
        /// </summary>
        [Description("分类字段")]
        public string Category { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        #endregion
    }
}
