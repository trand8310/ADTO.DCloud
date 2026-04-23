using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.DynamicEntityProperties
{
    [Table("ADTOSharpDynamicEntityProperties")]
    public class DynamicEntityProperty : Entity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="EntityFullName"/> property.
        /// </summary>
        public const int MaxEntityFullName = 256;
        
        [StringLength(MaxEntityFullName)]
        public string EntityFullName { get; set; }

        [Required]
        public Guid DynamicPropertyId { get; set; }

        public Guid? TenantId { get; set; }
        
        [ForeignKey("DynamicPropertyId")]
        public virtual DynamicProperty DynamicProperty { get; set; }

    }
}
