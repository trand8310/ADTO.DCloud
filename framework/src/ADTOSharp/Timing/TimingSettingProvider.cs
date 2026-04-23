using System.Collections.Generic;
using ADTOSharp.Configuration;
using ADTOSharp.Localization;

namespace ADTOSharp.Timing
{
    public class TimingSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(TimingSettingNames.TimeZone, "UTC", L("TimeZone"), scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider())
            };
        }

        private static LocalizableString L(string name)
        {
            return new LocalizableString(name, ADTOSharpConsts.LocalizationSourceName);
        }
    }
}
