using System;
using System.Threading.Tasks;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ADTOSharp.AspNetCore.ExceptionHandling;

public class DefaultADTOSharpAuthorizationExceptionHandler : IADTOSharpAuthorizationExceptionHandler, ITransientDependency
{
    public virtual async Task HandleAsync(ADTOSharpAuthorizationException exception, HttpContext httpContext)
    {
        var handlerOptions = httpContext.RequestServices.GetRequiredService<IOptions<ADTOSharpAuthorizationExceptionHandlerOptions>>().Value;
        var isAuthenticated = httpContext.User.Identity?.IsAuthenticated ?? false;
        var authenticationSchemeProvider = httpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

        AuthenticationScheme? scheme = null;

        if (!handlerOptions.AuthenticationScheme.IsNullOrWhiteSpace())
        {
            scheme = await authenticationSchemeProvider.GetSchemeAsync(handlerOptions.AuthenticationScheme!);
            if (scheme == null)
            {
                throw new ADTOSharpException($"No authentication scheme named {handlerOptions.AuthenticationScheme} was found.");
            }
        }
        else
        {
            if (isAuthenticated)
            {
                scheme = await authenticationSchemeProvider.GetDefaultForbidSchemeAsync();
                if (scheme == null)
                {
                    throw new ADTOSharpException($"There was no DefaultForbidScheme found.");
                }
            }
            else
            {
                scheme = await authenticationSchemeProvider.GetDefaultChallengeSchemeAsync();
                if (scheme == null)
                {
                    throw new ADTOSharpException($"There was no DefaultChallengeScheme found.");
                }
            }
        }

        var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name);
        if (handler == null)
        {
            throw new ADTOSharpException($"No handler of {scheme.Name} was found.");
        }

        if (isAuthenticated)
        {
            await handler.ForbidAsync(null);
        }
        else
        {
            await handler.ChallengeAsync(null);
        }
    }
}
