using System;

namespace ADTO.DCloud.DynamicEntityPropertyValues.Dto;

public class CleanValuesInput
{
    public Guid DynamicEntityPropertyId { get; set; }

    public string EntityId { get; set; }
}
