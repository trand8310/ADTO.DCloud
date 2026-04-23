using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.DynamicEntityProperties
{
    [Table("ADTOSharpDynamicProperties")]
    public class DynamicProperty : Entity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="MaxPropertyName"/> property.
        /// </summary>
        public const int MaxPropertyName = 256;
        
        [StringLength(MaxPropertyName)]
        public string PropertyName { get; set; }
        
        public string DisplayName { get; set; }

        public string InputType { get; set; }

        public string Permission { get; set; }
        
        public Guid? TenantId { get; set; }

        public virtual ICollection<DynamicPropertyValue> DynamicPropertyValues { get; set; }
    }
}
