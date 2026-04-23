using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.DataArea
{
    /// <summary>
    /// 行政区域表
    /// </summary>
    [Description("行政区域表"), Table("DataAreas")]
    public class DataArea : FullAuditedAggregateRoot<string>, IPassivable, IRemark, IDisplayOrder
    {
        /// <summary>
        /// 父级主键
        /// </summary>
        [Description("父级")]
        public string ParentId { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        [Description("区域编码")]
        public string AreaCode { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        [Description("区域名称")]
        public string AreaName { get; set; }

        /// <summary>
        /// 快速查询
        /// </summary>
        [Description("快速查询")]
        public string QuickQuery { get; set; }

        /// <summary>
        /// 简拼
        /// </summary>
        [Description("简拼")]
        public string SimpleSpelling { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        [Description("层次")]
        public int? Layer { get; set; }

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
        /// 排序
        /// </summary>
        [Description("排序")]
        public int DisplayOrder { get; set; }

    }
}
