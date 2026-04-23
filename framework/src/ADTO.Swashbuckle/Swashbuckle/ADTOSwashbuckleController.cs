using ADTOSharp.AspNetCore.Mvc.Controllers;
using ADTOSharp.Auditing;
using ADTOSharp.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
 
namespace ADTO.Swashbuckle;

[Area("ADTO")]
[Route("ADTO/Swashbuckle/[action]")]
[DisableAuditing]
[ApiExplorerSettings(IgnoreApi = true)]
public class ADTOSwashbuckleController : ADTOSharpController
{
    private readonly IAntiforgery _antiforgery;
    protected readonly IADTOSharpAntiForgeryManager _antiForgeryManager;


    public ADTOSwashbuckleController(IAntiforgery antiforgery, IADTOSharpAntiForgeryManager antiForgeryManager)
    {
        _antiforgery = antiforgery;
        _antiForgeryManager = antiForgeryManager;
    }
    public void GetToken()
    {
        _antiforgery.SetCookieTokenAndHeader(HttpContext);
    }

    public void SetCookie()
    {
        _antiForgeryManager.SetCookie(HttpContext);
    }
}
