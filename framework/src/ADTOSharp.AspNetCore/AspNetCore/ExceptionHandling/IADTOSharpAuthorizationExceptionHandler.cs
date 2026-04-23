using System.Threading.Tasks;
using ADTOSharp.Authorization;
using Microsoft.AspNetCore.Http;


namespace ADTOSharp.AspNetCore.ExceptionHandling;

public interface IADTOSharpAuthorizationExceptionHandler
{
    Task HandleAsync(ADTOSharpAuthorizationException exception, HttpContext httpContext);
}
