using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Authorization
{
    /// <summary>
    /// 用于授予/拒绝角色或用户的权限。
    /// </summary>
    [Table("ADTOSharpPermissions"), Description("权限信息")]
    public abstract class PermissionSetting : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="Name"/> field.
        /// </summary>
        public const int MaxNameLength = 128;

        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// Unique name of the permission.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Is this role granted for this permission.
        /// Default value: true.
        /// </summary>
        public virtual bool IsGranted { get; set; }

        /// <summary>
        /// Creates a new <see cref="PermissionSetting"/> entity.
        /// </summary>
        protected PermissionSetting()
        {
            IsGranted = true;
        }
    }
}
