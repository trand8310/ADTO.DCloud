using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ADTOSharp.HtmlSanitizer.Middleware;

public class ADTOSharpHtmlSanitizerMiddleware(
    RequestDelegate next,
    IMiddlewareHtmlSanitizerHelper htmlSanitizerHelper)
{
    public async Task Invoke(HttpContext httpContext)
    {
        if (!htmlSanitizerHelper.ShouldSanitizeContext(httpContext))
        {
            await next(httpContext);
            return;
        }

        await htmlSanitizerHelper.SanitizeContext(httpContext);
        await next(httpContext);
    }
}