using System;
using ADTOSharp.AspNetCore.Mvc.Controllers;
using ADTOSharp.Auditing;
using ADTOSharp.Web.Models;
using ADTOSharp.Web.Mvc.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ADTO.DCloud.Web.Controllers;

/// <summary>
/// 重定向错误控制器,配合前端使用,具体功能有待完善
/// </summary>
[DisableAuditing]
public class ErrorController : ADTOSharpController
{
    private readonly IErrorInfoBuilder _errorInfoBuilder;

    public ErrorController(IErrorInfoBuilder errorInfoBuilder)
    {
        _errorInfoBuilder = errorInfoBuilder;
    }

    public ActionResult Index(int statusCode = 0)
    {
        if (statusCode == 404)
        {
            return E404();
        }

        if (statusCode == 403)
        {
            return E403();
        }

        var exHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        var exception = exHandlerFeature != null
                            ? exHandlerFeature.Error
                            : new Exception("Unhandled exception!");

        return View(
            "Error",
            new ErrorViewModel(
                _errorInfoBuilder.BuildForException(exception),
                exception
            )
        );
    }
    
    public ActionResult E403()
    {
        return View("Error403");
    }

    public ActionResult E404()
    {
        return View("Error404");
    }
}