using System.Text;
using System.Threading.Tasks;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Web.Http;

namespace ADTOSharp.Web.Settings
{
    /// <summary>
    /// This class is used to build setting script.
    /// </summary>
    public class SettingScriptManager : ISettingScriptManager, ISingletonDependency
    {
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;
        private readonly IADTOSharpSession _adtoSession;
        private readonly IIocResolver _iocResolver;

        public SettingScriptManager(
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            IADTOSharpSession adtoSession,
            IIocResolver iocResolver)
        {
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
            _adtoSession = adtoSession;
            _iocResolver = iocResolver;
        }

        public async Task<string> GetScriptAsync()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    adto.setting = adto.setting || {};");
            script.AppendLine("    adto.setting.values = {");

            var settingDefinitions = _settingDefinitionManager
                .GetAllSettingDefinitions();

            var added = 0;

            using (var scope = _iocResolver.CreateScope())
            {
                foreach (var settingDefinition in settingDefinitions)
                {
                    if (!await settingDefinition.ClientVisibilityProvider.CheckVisible(scope))
                    {
                        continue;
                    }

                    if (added > 0)
                    {
                        script.AppendLine(",");
                    }
                    else
                    {
                        script.AppendLine();
                    }

                    var settingValue = await _settingManager.GetSettingValueAsync(settingDefinition.Name);

                    script.Append("        '" +
                                  HttpEncode.JavaScriptStringEncode(settingDefinition.Name) + "': " +
                                  (settingValue == null ? "null" : "'" + HttpEncode.JavaScriptStringEncode(settingValue) + "'"));

                    ++added;
                }
            }

            script.AppendLine();
            script.AppendLine("    };");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}