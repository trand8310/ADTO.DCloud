using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Uow;

public class ADTOSharpUowActionFilter : IAsyncActionFilter, ITransientDependency
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IADTOSharpAspNetCoreConfiguration _aspnetCoreConfiguration;
    private readonly IUnitOfWorkDefaultOptions _unitOfWorkDefaultOptions;

    public ADTOSharpUowActionFilter(
        IUnitOfWorkManager unitOfWorkManager,
        IADTOSharpAspNetCoreConfiguration aspnetCoreConfiguration,
        IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _aspnetCoreConfiguration = aspnetCoreConfiguration;
        _unitOfWorkDefaultOptions = unitOfWorkDefaultOptions;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionDescriptor.IsControllerAction())
        {
            await next();
            return;
        }

        var unitOfWorkAttr = _unitOfWorkDefaultOptions
            .GetUnitOfWorkAttributeOrNull(context.ActionDescriptor.GetMethodInfo()) ??
            _aspnetCoreConfiguration.DefaultUnitOfWorkAttribute;

        if (unitOfWorkAttr.IsDisabled)
        {
            await next();
            return;
        }

        using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
        {
            var result = await next();
            if (result.Exception == null || result.ExceptionHandled)
            {
                await uow.CompleteAsync();
            }
        }
    }
}