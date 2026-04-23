using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.AreaBase
{
    /// <summary>
    /// 地区表（亚洲类）
    /// </summary>
    [Table("Base_Area")]
    public class Base_Area : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IRemark, IPassivable
    {
        /// <summary>
        /// 父级
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名称(全称)
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否有效、启用
        /// </summary>

        public bool IsActive { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}
