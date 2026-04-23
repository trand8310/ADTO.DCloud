using ADTOSharp.HtmlSanitizer.ActionFilter;
using ADTOSharp.HtmlSanitizer.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace ADTOSharp.HtmlSanitizer;

public static class ADTOSharpHtmlSanitizerExtensions
{
    public static void AddADTOSharpHtmlSanitizer(this MvcOptions options)
    {
        options.Filters.AddService(typeof(ADTOSharpHtmlSanitizerActionFilter));
    }
        
    public static IApplicationBuilder UseADTOSharpHtmlSanitizer(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ADTOSharpHtmlSanitizerMiddleware>();
    }
}