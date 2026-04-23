using System.Text;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Web.Sessions
{
    public class SessionScriptManager : ISessionScriptManager, ITransientDependency
    {
        public IADTOSharpSession ADTOSharpSession { get; set; }

        public SessionScriptManager()
        {
            ADTOSharpSession = NullADTOSharpSession.Instance;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            script.AppendLine("    adto.session = adto.session || {};");
            script.AppendLine("    adto.session.userId = " + (ADTOSharpSession.UserId.HasValue ? ADTOSharpSession.UserId.Value.ToString() : "null") + ";");
            script.AppendLine("    adto.session.tenantId = " + (ADTOSharpSession.TenantId.HasValue ? ADTOSharpSession.TenantId.Value.ToString() : "null") + ";");
            script.AppendLine("    adto.session.impersonatorUserId = " + (ADTOSharpSession.ImpersonatorUserId.HasValue ? ADTOSharpSession.ImpersonatorUserId.Value.ToString() : "null") + ";");
            script.AppendLine("    adto.session.impersonatorTenantId = " + (ADTOSharpSession.ImpersonatorTenantId.HasValue ? ADTOSharpSession.ImpersonatorTenantId.Value.ToString() : "null") + ";");
            script.AppendLine("    adto.session.multiTenancySide = " + ((int)ADTOSharpSession.MultiTenancySide) + ";");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}