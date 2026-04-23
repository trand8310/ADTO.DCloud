using ADTOSharp.Web.Api.Modeling;

namespace ADTOSharp.Web.Api.ProxyScripting.Generators
{
    public interface IProxyScriptGenerator
    {
        string CreateScript(ApplicationApiDescriptionModel model);
    }
}