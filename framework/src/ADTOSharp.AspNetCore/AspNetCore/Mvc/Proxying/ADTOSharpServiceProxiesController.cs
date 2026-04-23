using ADTOSharp.AspNetCore.Mvc.Controllers;
using ADTOSharp.Auditing;
using ADTOSharp.Web.Api.ProxyScripting;
using ADTOSharp.Web.Minifier;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ADTOSharp.AspNetCore.Mvc.Proxying;

[DontWrapResult]
[DisableAuditing]
public class ADTOSharpServiceProxiesController : ADTOSharpController
{
    private readonly IApiProxyScriptManager _proxyScriptManager;
    private readonly IJavaScriptMinifier _javaScriptMinifier;

    public ADTOSharpServiceProxiesController(IApiProxyScriptManager proxyScriptManager,
        IJavaScriptMinifier javaScriptMinifier)
    {
        _proxyScriptManager = proxyScriptManager;
        _javaScriptMinifier = javaScriptMinifier;
    }

    [Produces("application/x-javascript")]
    public ContentResult GetAll(ApiProxyGenerationModel model)
    {
        var script = _proxyScriptManager.GetScript(model.CreateOptions());
        return Content(model.Minify ? _javaScriptMinifier.Minify(script) : script, "application/x-javascript");
    }
}