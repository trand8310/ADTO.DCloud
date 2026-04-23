using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    /// <summary>
    /// 获取详情
    /// </summary>
    [AutoMap(typeof(ExcelImport))]
    public class ExcelImportDetailDto : EntityDto<Guid>
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
        /// 字段信息
        /// </summary>
        public List<ExcelImportFieldDto> FieldDtos { get; set; }
    }
}
