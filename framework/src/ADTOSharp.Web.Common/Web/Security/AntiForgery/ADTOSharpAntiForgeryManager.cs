using System;
using ADTOSharp.Dependency;
using Castle.Core.Logging;

namespace ADTOSharp.Web.Security.AntiForgery
{
    public class ADTOSharpAntiForgeryManager : IADTOSharpAntiForgeryManager, IADTOSharpAntiForgeryValidator, ITransientDependency
    {
        public ILogger Logger { protected get; set; }

        public IADTOSharpAntiForgeryConfiguration Configuration { get; }

        public ADTOSharpAntiForgeryManager(IADTOSharpAntiForgeryConfiguration configuration)
        {
            Configuration = configuration;
            Logger = NullLogger.Instance;
        }

        public virtual string GenerateToken()
        {
            return Guid.NewGuid().ToString("D");
        }

        public virtual bool IsValid(string cookieValue, string tokenValue)
        {
            return cookieValue == tokenValue;
        }

        public virtual void SetCookie()
        {


        }
    }
}