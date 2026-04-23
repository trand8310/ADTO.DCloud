using System.Collections.Generic;
using System.Linq;
using ADTOSharp.Configuration;
using ADTOSharp.Configuration.Startup;

namespace ADTO.DCloud.EntityHistory;

/// <summary>
/// 实体的历史变更配置提供者
/// </summary>
public class EntityHistoryConfigProvider : ICustomConfigProvider
{
    private readonly IADTOSharpStartupConfiguration _startupConfiguration;

    public EntityHistoryConfigProvider(IADTOSharpStartupConfiguration startupConfiguration)
    {
        _startupConfiguration = startupConfiguration;
    }

    public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
    {
        if (!_startupConfiguration.EntityHistory.IsEnabled)
        {
            return new Dictionary<string, object>
            {
                {
                    EntityHistoryHelper.EntityHistoryConfigurationName,
                    new EntityHistoryUiSetting{
                        IsEnabled = false
                    }
                }
            };
        }

        var entityHistoryEnabledEntities = new List<string>();

        foreach (var type in EntityHistoryHelper.TrackedTypes)
        {
            if (_startupConfiguration.EntityHistory.Selectors.Any(s => s.Predicate(type)))
            {
                entityHistoryEnabledEntities.Add(type.FullName);
            }
        }

        return new Dictionary<string, object>
        {
            {
                EntityHistoryHelper.EntityHistoryConfigurationName,
                new EntityHistoryUiSetting {
                    IsEnabled = true,
                    EnabledEntities = entityHistoryEnabledEntities
                }
            }
        };
    }
}
