using System;
using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Results.Wrapping;
using ADTOSharp.Dependency;
using ADTOSharp.Reflection;
using ADTOSharp.Web.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results;

public class ADTOSharpResultPageFilter : IAsyncPageFilter, ITransientDependency
{
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;
    private readonly IADTOSharpActionResultWrapperFactory _actionResultWrapperFactory;
    private readonly IADTOSharpWebCommonModuleConfiguration _adtoWebCommonModuleConfiguration;

    public ADTOSharpResultPageFilter(IADTOSharpAspNetCoreConfiguration configuration,
        IADTOSharpActionResultWrapperFactory actionResultWrapperFactory,
        IADTOSharpWebCommonModuleConfiguration adtoWebCommonModuleConfiguration)
    {
        _configuration = configuration;
        _actionResultWrapperFactory = actionResultWrapperFactory;
        _adtoWebCommonModuleConfiguration = adtoWebCommonModuleConfiguration;
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (context.HandlerMethod == null)
        {
            await next();
            return;
        }

        var pageHandlerExecutedContext = await next();

        var methodInfo = context.HandlerMethod.MethodInfo;

        /*
        * Here is the check order,
        * 1) Configuration
        * 2) Attribute
        */

        var displayUrl = context.HttpContext.Request.GetDisplayUrl();

        if (_adtoWebCommonModuleConfiguration.WrapResultFilters.HasFilterForWrapOnSuccess(displayUrl, out var wrapOnSuccess))
        {
            //there is a configuration for that method use configuration
            if (!wrapOnSuccess)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(pageHandlerExecutedContext).Wrap(pageHandlerExecutedContext);
            return;
        }

        var wrapResultAttribute =
            ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                methodInfo,
                _configuration.DefaultWrapResultAttribute
            );

        if (!wrapResultAttribute.WrapOnSuccess)
        {
            return;
        }

        _actionResultWrapperFactory.CreateFor(pageHandlerExecutedContext).Wrap(pageHandlerExecutedContext);
    }

}