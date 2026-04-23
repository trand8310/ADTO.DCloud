using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTO.DCloud.Project
{
    /// <summary>
    /// 项目基本信息
    /// </summary>
    [Description("项目基本信息"), Table("ProjectInfos")]
    public class ProjectInfo : FullAuditedAggregateRoot<Guid>, IPassivable, IMayHaveTenant, IRemark, IDisplayOrder
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [Description("项目名称")]
        public string Name { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        [Description("项目编码")]
        public string Code { get; set; }

        /// <summary>
        /// 项目规模
        /// </summary>
        [Description("项目规模")]
        public decimal Scale { get; set; }

        /// <summary>
        /// 项目金额
        /// </summary>
        [Description("项目金额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("项目开始时间")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Description("项目结束时间")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Description("是否激活")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 项目地址
        /// </summary>
        [Description("项目地址")]
        public string Address{ get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }
    }
}
