using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.DatabaseManager.ImportTableSources.Dto
{
    /// <summary>
    /// 添加导入表数据源
    /// </summary>
    [AutoMapTo(typeof(ImportTableSource))]
    public class CreateImportTableSourcesDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表说明描述
        /// </summary>
        public string Desc { get; set; }
    }
}
