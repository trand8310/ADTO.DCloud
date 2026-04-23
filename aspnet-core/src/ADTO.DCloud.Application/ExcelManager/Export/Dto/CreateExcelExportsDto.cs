using ADTO.DCloud.ExcelManager.Import.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    /// <summary>
    /// 新增Excel 导出配置
    /// </summary>
    [AutoMapTo(typeof(ExcelExport))]
    public class CreateExcelExportsDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 唯一编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 业务服务完整名
        /// 例：ADTO.DCloud.IUserAppService
        /// </summary>
        public string ServiceFullName { get; set; }

        /// <summary>
        /// 调用方法名
        /// 例：GetAllListAsync
        /// </summary>
        public string MethodName { get; set; }

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

        /// <summary>
        /// 字段信息
        /// </summary>
        public List<CreateExcelExportFieldDto> FieldDtos { get; set; }

        ///// <summary>
        ///// 参数信息
        ///// </summary>
        //public List<CreateExcelExportParamsDto> ParamsDtos { get; set; }
    }
}
