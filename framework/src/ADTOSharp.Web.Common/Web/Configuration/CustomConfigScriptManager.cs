using System.Text;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Json;

namespace ADTOSharp.Web.Configuration
{
    public class CustomConfigScriptManager : ICustomConfigScriptManager, ITransientDependency
    {
        private readonly IADTOSharpStartupConfiguration _adtoStartupConfiguration;

        public CustomConfigScriptManager(IADTOSharpStartupConfiguration adtoStartupConfiguration)
        {
            _adtoStartupConfiguration = adtoStartupConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(adto){");
            script.AppendLine();

            script.AppendLine("    adto.custom = " + _adtoStartupConfiguration.GetCustomConfig().ToJsonString());

            script.AppendLine();
            script.Append("})(adto);");

            return script.ToString();
        }
    }
}