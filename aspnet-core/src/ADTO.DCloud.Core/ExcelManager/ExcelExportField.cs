using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.ExcelManager
{
    /// <summary>
    /// Excel 导出字段设置表
    /// </summary>
    [Description("Excel导出配置字段表"), Table("ExcelExportField")]
    public class ExcelExportField : Entity<Guid>, ICreationAudited, IMayHaveTenant, IDisplayOrder
    {
        /// <summary>
        ///  配置主表Id
        /// </summary>
        [ForeignKey("ExcelExport")]
        [Description("配置主表Id")]
        public Guid ExportId { get; set; }
        public virtual ExcelExport Export { get; set; }

        /// <summary>
        /// 表字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Excel列名
        /// </summary>
        public string ColName { get; set; }

        /// <summary>
        /// 格式化类型 （字典外键暂时不考虑，接口会把数据处理好）
        /// 0=不格式化 2=日期 3=Boolean
        /// </summary>
        public int FormatType { get; set; }

        /// <summary>
        /// 格式化参数
        /// 日期(yyyy-MM-dd) 
        /// Boolean（是、否）
        /// </summary>
        public string FormatConfig { get; set; }

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

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
