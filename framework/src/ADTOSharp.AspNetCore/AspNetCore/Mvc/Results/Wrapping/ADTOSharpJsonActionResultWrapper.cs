using System;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results.Wrapping;

public class ADTOSharpJsonActionResultWrapper : IADTOSharpActionResultWrapper
{
    public void Wrap(FilterContext context)
    {
        JsonResult jsonResult = null;

        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                jsonResult = resultExecutingContext.Result as JsonResult;
                break;

            case PageHandlerExecutedContext pageHandlerExecutedContext:
                jsonResult = pageHandlerExecutedContext.Result as JsonResult;
                break;
        }

        if (jsonResult == null)
        {
            throw new ArgumentException("Action Result should be JsonResult!");
        }

        if (!(jsonResult.Value is AjaxResponseBase))
        {
            jsonResult.Value = new AjaxResponse(jsonResult.Value);
        }
    }
}