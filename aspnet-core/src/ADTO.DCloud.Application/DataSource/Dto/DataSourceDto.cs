using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using System;

namespace ADTO.DCloud.DataSource.Dto
{
    /// <summary>
    /// 数据源
    /// </summary>
    [AutoMap(typeof(DataSource))]
    public class DataSourceDto : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库主键
        /// </summary>
        public string DbId { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 1.表格 2.代码 3.关联 0或者其他sql语句
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 分类字段
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
