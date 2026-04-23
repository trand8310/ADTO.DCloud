using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    /// <summary>
    /// 添加Excel导出栏位配置表
    /// </summary>
    [AutoMapTo(typeof(ExcelExportField))]
    public class CreateExcelExportFieldDto:EntityDto<Guid>
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
        /// 0=不格式化 1=日期 2=Boolean
        /// </summary>
        public int FormatType { get; set; }

        /// <summary>
        /// 格式化参数
        /// 日期(yyyy-MM-dd) 
        /// Boolean（是、否）
        /// </summary>
        public string FormatConfig { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
