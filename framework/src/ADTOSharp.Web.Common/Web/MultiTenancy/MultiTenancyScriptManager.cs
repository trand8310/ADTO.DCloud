using System;
using System.Globalization;
using System.Text;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Web.MultiTenancy
{
    public class MultiTenancyScriptManager : IMultiTenancyScriptManager, ITransientDependency
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public MultiTenancyScriptManager(IMultiTenancyConfig multiTenancyConfig)
        {
            _multiTenancyConfig = multiTenancyConfig;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(adto){");
            script.AppendLine();

            script.AppendLine("    adto.multiTenancy = adto.multiTenancy || {};");
            script.AppendLine("    adto.multiTenancy.isEnabled = " + _multiTenancyConfig.IsEnabled.ToString().ToLowerInvariant() + ";");

            script.AppendLine();
            script.Append("})(adto);");

            return script.ToString();
        }
    }
}