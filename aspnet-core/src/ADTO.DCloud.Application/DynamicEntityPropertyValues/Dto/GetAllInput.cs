using System;

namespace ADTO.DCloud.DynamicEntityPropertyValues.Dto;

public class GetAllInput
{
    public string EntityId { get; set; }

    public Guid PropertyId { get; set; }
}
