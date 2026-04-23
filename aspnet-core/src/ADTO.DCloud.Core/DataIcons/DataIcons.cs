using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.DataIcons
{
    /// <summary>
    /// 系统图标表
    /// </summary>
    [Description("系统图标表"), Table("DataIcons")]
    public class DataIcons : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// 高
        /// </summary>
        [Description("高")]
        public int? Height { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        [Description("宽")]
        public int? Width { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Description("编码")]
        public string Code { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        [Description("分类")]
        public string Category { get; set; }

        /// <summary>
        /// svg数据
        /// </summary>
        [Description("svg数据")]
        public string Body { get; set; }
    }
}
