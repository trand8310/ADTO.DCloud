using System.Threading.Tasks;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Http;

namespace ADTOSharp.HtmlSanitizer.Middleware;

public interface IMiddlewareHtmlSanitizerHelper : ISingletonDependency
{
    bool ShouldSanitizeContext(HttpContext context);
        
    Task SanitizeContext(HttpContext context);
}