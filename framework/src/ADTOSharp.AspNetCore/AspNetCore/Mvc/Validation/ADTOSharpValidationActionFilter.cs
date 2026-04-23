using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTOSharp.Aspects;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Validation;

public class ADTOSharpValidationActionFilter : IAsyncActionFilter, ITransientDependency
{
    private readonly IIocResolver _iocResolver;
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;

    public ADTOSharpValidationActionFilter(IIocResolver iocResolver, IADTOSharpAspNetCoreConfiguration configuration)
    {
        _iocResolver = iocResolver;
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_configuration.IsValidationEnabledForControllers || !context.ActionDescriptor.IsControllerAction())
        {
            await next();
            return;
        }

        using (ADTOSharpCrossCuttingConcerns.Applying(context.Controller, ADTOSharpCrossCuttingConcerns.Validation))
        {
            using (var validator = _iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
            {
                validator.Object.Initialize(context);
                validator.Object.Validate();
            }

            await next();
        }
    }
}