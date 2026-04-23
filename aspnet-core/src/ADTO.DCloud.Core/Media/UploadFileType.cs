using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Media
{
    /// <summary>
    /// 所属文件夹类别
    /// </summary>
    [Description("所属文件夹类别"), Table("UploadFileTypes")]
    public class UploadFileType : FullAuditedEntity<Guid>, IMayHaveTenant, IDisplayOrder, IPassivable
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 表实体名称(类名区分是项目、客户。。。)
        /// </summary>
        public string ProjectClassName { get; set; }

        /// <summary>
        /// 实体Id(例如项目Id、客户Id)
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
