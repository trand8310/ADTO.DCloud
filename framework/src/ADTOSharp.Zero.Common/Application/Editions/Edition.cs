using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Application.Editions
{
    /// <summary>
    /// 系统版本
    /// </summary>
    [Table("Editions"), Description("系统版本")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class Edition : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// Maximum length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 32;

        /// <summary>
        /// Maximum length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxDisplayNameLength = 64;

        /// <summary>
        /// Unique name of this edition.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name of this edition.
        /// </summary>
        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public Edition()
        {
            Name = Guid.NewGuid().ToString("N");
        }

        public Edition(string displayName)
            : this()
        {
            DisplayName = displayName;
        }
    }
}