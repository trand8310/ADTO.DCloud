using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results.Wrapping;

public class ADTOSharpEmptyActionResultWrapper : IADTOSharpActionResultWrapper
{
    public void Wrap(FilterContext context)
    {
        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                resultExecutingContext.Result = new ObjectResult(new AjaxResponse());
                return;

            case PageHandlerExecutedContext pageHandlerExecutedContext:
                pageHandlerExecutedContext.Result = new ObjectResult(new AjaxResponse());
                return;
        }
    }
}