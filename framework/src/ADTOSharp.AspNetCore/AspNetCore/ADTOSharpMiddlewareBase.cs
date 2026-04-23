using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ADTOSharp.AspNetCore.Middleware;

public abstract class ADTOSharpMiddlewareBase : IMiddleware
{
    protected virtual Task<bool> ShouldSkipAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        var disableAbpFeaturesAttribute = controllerActionDescriptor?.ControllerTypeInfo.GetCustomAttribute<DisableADTOSharpFeaturesAttribute>();
        return Task.FromResult(disableAbpFeaturesAttribute != null && disableAbpFeaturesAttribute.DisableMiddleware);
    }

    public abstract Task InvokeAsync(HttpContext context, RequestDelegate next);
}
