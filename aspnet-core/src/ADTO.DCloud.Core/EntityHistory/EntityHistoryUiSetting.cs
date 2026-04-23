using System.Collections.Generic;

namespace ADTO.DCloud.EntityHistory;

public class EntityHistoryUiSetting
{
    public bool IsEnabled { get; set; }

    public List<string> EnabledEntities { get; set; }
}