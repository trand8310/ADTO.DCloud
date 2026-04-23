using System.Text;
using ADTOSharp.Dependency;
using ADTOSharp.Web.Security.AntiForgery;

namespace ADTOSharp.Web.Security
{
    internal class SecurityScriptManager : ISecurityScriptManager, ITransientDependency
    {
        private readonly IADTOSharpAntiForgeryConfiguration _adtoAntiForgeryConfiguration;

        public SecurityScriptManager(IADTOSharpAntiForgeryConfiguration adtoAntiForgeryConfiguration)
        {
            _adtoAntiForgeryConfiguration = adtoAntiForgeryConfiguration;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine("    adto.security.antiForgery.tokenCookieName = '" + _adtoAntiForgeryConfiguration.TokenCookieName + "';");
            script.AppendLine("    adto.security.antiForgery.tokenHeaderName = '" + _adtoAntiForgeryConfiguration.TokenHeaderName + "';");
            script.Append("})();");

            return script.ToString();
        }
    }
}
