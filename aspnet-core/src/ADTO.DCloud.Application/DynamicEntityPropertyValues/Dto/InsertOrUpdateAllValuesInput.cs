using System;
using System.Collections.Generic;

namespace ADTO.DCloud.DynamicEntityPropertyValues.Dto;

public class InsertOrUpdateAllValuesInput
{
    public List<InsertOrUpdateAllValuesInputItem> Items { get; set; }
}

public class InsertOrUpdateAllValuesInputItem
{
    public string EntityId { get; set; }

    public Guid DynamicEntityPropertyId { get; set; }

    public List<string> Values { get; set; }
}
