using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTOSharp.Configuration
{
    /// <summary>
    /// 存储租户或用户的设置。
    /// </summary>
    [Table("ADTOSharpSettings"),Description("系统设置")]
    public class Setting : AuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 256;
     
        /// <summary>
        /// TenantId for this setting.
        /// TenantId is null if this setting is not Tenant level.
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// UserId for this setting.
        /// UserId is null if this setting is not user level.
        /// </summary>
        public virtual Guid? UserId { get; set; }

        /// <summary>
        /// Unique name of the setting.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="Setting"/> object.
        /// </summary>
        public Setting()
        {

        }

        /// <summary>
        /// Creates a new <see cref="Setting"/> object.
        /// </summary>
        /// <param name="tenantId">TenantId for this setting</param>
        /// <param name="userId">UserId for this setting</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="value">Value of the setting</param>
        public Setting(Guid? tenantId, Guid? userId, string name, string value)
        {
            TenantId = tenantId;
            UserId = userId;
            Name = name;
            Value = value;
        }
    }
}