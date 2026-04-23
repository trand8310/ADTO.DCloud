using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using System;

namespace ADTO.DCloud.CodeTable.Dto
{
    /// <summary>
    /// (数据库表信息)表的实体
    /// </summary>
    [AutoMap(typeof(CodeTable))]
    public class CodeTableDto : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据库连接外键【lr_base_databaselink】
        /// </summary>
        public string DbId { get; set; }

        /// <summary>
        /// 是否锁表【1是，0否】
        /// </summary>
        public int? IsLock { get; set; }

        /// <summary>
        /// 模型与数据库间的同步关系 1 同步 0 未同步
        /// </summary>
        public int? State { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
