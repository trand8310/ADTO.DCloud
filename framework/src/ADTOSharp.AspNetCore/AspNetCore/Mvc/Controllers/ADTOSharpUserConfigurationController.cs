using System.Threading.Tasks;
using ADTOSharp.Web.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace ADTOSharp.AspNetCore.Mvc.Controllers;

public class ADTOSharpUserConfigurationController : ADTOSharpController
{
    private readonly ADTOSharpUserConfigurationBuilder _adtoUserConfigurationBuilder;

    public ADTOSharpUserConfigurationController(ADTOSharpUserConfigurationBuilder adtoUserConfigurationBuilder)
    {
        _adtoUserConfigurationBuilder = adtoUserConfigurationBuilder;
    }

    protected async Task<JsonResult> GetAll()
    {
        var userConfig = await _adtoUserConfigurationBuilder.GetAll();
        return Json(userConfig);
    }
}