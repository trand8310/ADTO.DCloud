using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    /// <summary>
    /// Excel导出配置详情
    /// </summary>
    [AutoMap(typeof(ExcelExport))]
    public class ExcelExportsDetailDto
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
        /// 字段信息
        /// </summary>
        public List<ExcelExportFieldDto> FieldDtos { get; set; }

        /// <summary>
        /// 参数信息
        /// </summary>
        public List<ExcelExportParamsDto> ParamsDtos { get; set; }
    }
}
