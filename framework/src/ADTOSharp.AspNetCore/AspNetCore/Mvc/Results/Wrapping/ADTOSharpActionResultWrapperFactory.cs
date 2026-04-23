using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results.Wrapping;

public class ADTOSharpActionResultWrapperFactory : IADTOSharpActionResultWrapperFactory
{
    public IADTOSharpActionResultWrapper CreateFor(FilterContext context)
    {
        Check.NotNull(context, nameof(context));

        switch (context)
        {
            case ResultExecutingContext resultExecutingContext when resultExecutingContext.Result is ObjectResult:
                return new ADTOSharpObjectActionResultWrapper();

            case ResultExecutingContext resultExecutingContext when resultExecutingContext.Result is JsonResult:
                return new ADTOSharpJsonActionResultWrapper();

            case ResultExecutingContext resultExecutingContext when resultExecutingContext.Result is EmptyResult:
                return new ADTOSharpEmptyActionResultWrapper();

            case PageHandlerExecutedContext pageHandlerExecutedContext when pageHandlerExecutedContext.Result is ObjectResult:
                return new ADTOSharpObjectActionResultWrapper();

            case PageHandlerExecutedContext pageHandlerExecutedContext when pageHandlerExecutedContext.Result is JsonResult:
                return new ADTOSharpJsonActionResultWrapper();

            case PageHandlerExecutedContext pageHandlerExecutedContext when pageHandlerExecutedContext.Result is EmptyResult:
                return new ADTOSharpEmptyActionResultWrapper();

            default:
                return new NullADTOSharpActionResultWrapper();
        }
    }
}