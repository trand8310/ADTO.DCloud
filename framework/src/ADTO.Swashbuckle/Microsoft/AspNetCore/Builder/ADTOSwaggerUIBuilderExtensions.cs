using System;
using ADTO.Swashbuckle;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.AspNetCore.Builder;

public static class ADTOSwaggerUIBuilderExtensions
{
    public static IApplicationBuilder UseADTOSwaggerUI(
        this IApplicationBuilder app,
        Action<SwaggerUIOptions>? setupAction = null)
    {
        var resolver = app.ApplicationServices.GetService<ISwaggerHtmlResolver>();

        return app.UseSwaggerUI(options =>
        {
            options.InjectJavascript("ui/adto.js");
            options.IndexStream = () => resolver?.Resolver();

            setupAction?.Invoke(options);
        });
    }
}
