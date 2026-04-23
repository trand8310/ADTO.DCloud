using System;
using System.Collections.Generic;
using ADTOSharp.UI.Inputs;

namespace ADTO.DCloud.DynamicEntityPropertyValues.Dto;

public class GetAllDynamicEntityPropertyValuesOutput
{
    public List<GetAllDynamicEntityPropertyValuesOutputItem> Items { get; set; }

    public GetAllDynamicEntityPropertyValuesOutput()
    {
        Items = new List<GetAllDynamicEntityPropertyValuesOutputItem>();
    }
}

public class GetAllDynamicEntityPropertyValuesOutputItem
{
    public Guid DynamicEntityPropertyId { get; set; }

    public string PropertyName { get; set; }

    public IInputType InputType { get; set; }

    public List<string> SelectedValues { get; set; }

    public List<string> AllValuesInputTypeHas { get; set; }
}
