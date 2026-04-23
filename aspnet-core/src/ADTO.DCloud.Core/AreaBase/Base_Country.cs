using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.AreaBase
{
    /// <summary>
    /// 国家表
    /// </summary>
    [Table("Base_Country")]
    public class Base_Country : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IRemark, IPassivable
    {
        /// <summary>
        /// 区域Id
        /// </summary>
        public Guid AreaId { get; set; }

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
        /// 是否有效、启用
        /// </summary>
        [Description("是否有效、启用")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
