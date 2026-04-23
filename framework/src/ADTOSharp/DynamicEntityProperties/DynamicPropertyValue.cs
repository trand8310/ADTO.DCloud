using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.DynamicEntityProperties
{
    [Table("ADTOSharpDynamicPropertyValues")]
    public class DynamicPropertyValue : Entity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// Value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        public Guid? TenantId { get; set; }

        public Guid DynamicPropertyId { get; set; }

        [ForeignKey("DynamicPropertyId")]
        public virtual DynamicProperty DynamicProperty { get; set; }

        public DynamicPropertyValue()
        {
        }

        public DynamicPropertyValue(DynamicProperty dynamicProperty, string value, Guid? tenantId)
        {
            Value = value;
            TenantId = tenantId;
            DynamicPropertyId = dynamicProperty.Id;
        }
    }
}
