using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.DataSource.Dto
{
    /// <summary>
    /// 添加数据源
    /// </summary>
    [AutoMapTo(typeof(DataSource))]
    public class    CreateDataSourceInputDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 编号
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
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
