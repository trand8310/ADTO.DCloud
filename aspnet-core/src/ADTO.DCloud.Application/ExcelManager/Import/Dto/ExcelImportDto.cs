using System;
using System.ComponentModel;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    /// <summary>
    ///  excel 导入配置表
    /// </summary>
    [AutoMap(typeof(ExcelImport))]
    public class ExcelImportDto : EntityDto<Guid>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string DbTable { get; set; }

        /// <summary>
        /// 错误处理机制0终止,1跳过  默认0
        /// </summary>
        public int? ErrorType { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Description("是否有效")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        [Description("租户")]
        public Guid? TenantId { get; set; }
    }
}
