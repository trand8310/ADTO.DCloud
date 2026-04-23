using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;

namespace ADTOSharp.DynamicEntityProperties
{
    [Table("ADTOSharpDynamicEntityPropertyValues")]
    public class DynamicEntityPropertyValue : Entity<Guid>, IMayHaveTenant
    {
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        public string EntityId { get; set; }

        public Guid DynamicEntityPropertyId { get; set; }

        public virtual DynamicEntityProperty DynamicEntityProperty { get; set; }

        public Guid? TenantId { get; set; }

        public DynamicEntityPropertyValue()
        {

        }

        public DynamicEntityPropertyValue(DynamicEntityProperty dynamicEntityProperty, string entityId, string value, Guid? tenantId)
        {
            DynamicEntityPropertyId = dynamicEntityProperty.Id;
            EntityId = entityId;
            Value = value;
            TenantId = tenantId;
        }
    }
}
