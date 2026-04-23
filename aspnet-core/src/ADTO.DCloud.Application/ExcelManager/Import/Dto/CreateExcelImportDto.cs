using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    /// <summary>
    /// 添加excel 导入配置表
    /// </summary>
    [AutoMapTo(typeof(ExcelImport))]
    public class CreateExcelImportDto : EntityDto<Guid?>
    {
        #region 配置主表信息
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage ="名称不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        public string Code { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
        [Required(ErrorMessage = "表名称不能为空")]
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

        #endregion

        /// <summary>
        /// 字段信息
        /// </summary>
        public List<CreateExcelImportFieldDto> FieldDtos { get; set; }
    }
}
