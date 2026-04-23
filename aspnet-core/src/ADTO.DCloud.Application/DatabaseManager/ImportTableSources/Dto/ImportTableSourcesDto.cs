using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.DatabaseManager.ImportTableSources.Dto
{
    /// <summary>
    /// 数据表管理（导入表源表）
    /// </summary>
    [AutoMap(typeof(ImportTableSource))]
    public class ImportTableSourcesDto : FullAuditedEntityDto<Guid>
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
