using System.Threading.Tasks;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.HtmlSanitizer.ActionFilter;

public class ADTOSharpHtmlSanitizerActionFilter(IActionFilterHtmlSanitizerHelper htmlSanitizerHelper)
    : IAsyncActionFilter, ISingletonDependency
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!htmlSanitizerHelper.ShouldSanitizeContext(context))
        {
            await next();
            return;
        }

        htmlSanitizerHelper.SanitizeContext(context);
        await next();
    }
}