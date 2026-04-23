using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    /// <summary>
    /// Excel导出栏位配置表
    /// </summary>
    [AutoMap(typeof(ExcelExportField))]
    public class ExcelExportFieldDto:EntityDto<Guid>
    {
        /// <summary>
        ///  配置主表Id
        /// </summary>
        public Guid ExportId { get; set; }

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
