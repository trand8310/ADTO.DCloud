using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.HtmlSanitizer.ActionFilter;

public interface IActionFilterHtmlSanitizerHelper : ISingletonDependency
{
    bool ShouldSanitizeContext(ActionExecutingContext actionExecutingContext);
        
    void SanitizeContext(ActionExecutingContext context);
}