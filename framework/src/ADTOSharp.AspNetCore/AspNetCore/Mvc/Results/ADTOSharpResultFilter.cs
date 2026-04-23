using System;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.AspNetCore.Mvc.Results.Wrapping;
using ADTOSharp.Dependency;
using ADTOSharp.Reflection;
using ADTOSharp.Web.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Results;

public class ADTOSharpResultFilter : IResultFilter, ITransientDependency
{
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;
    private readonly IADTOSharpActionResultWrapperFactory _actionResultWrapperFactory;
    private readonly IADTOSharpWebCommonModuleConfiguration _adtoWebCommonModuleConfiguration;

    public ADTOSharpResultFilter(IADTOSharpAspNetCoreConfiguration configuration,
        IADTOSharpActionResultWrapperFactory actionResultWrapper,
        IADTOSharpWebCommonModuleConfiguration adtoWebCommonModuleConfiguration)
    {
        _configuration = configuration;
        _actionResultWrapperFactory = actionResultWrapper;
        _adtoWebCommonModuleConfiguration = adtoWebCommonModuleConfiguration;
    }

    public virtual void OnResultExecuting(ResultExecutingContext context)
    {
        if (!context.ActionDescriptor.IsControllerAction())
        {
            return;
        }

        var methodInfo = context.ActionDescriptor.GetMethodInfo();

        var displayUrl = context.HttpContext.Request.GetDisplayUrl();
        if (_adtoWebCommonModuleConfiguration.WrapResultFilters.HasFilterForWrapOnSuccess(displayUrl, out var wrapOnSuccess))
        {
            if (!wrapOnSuccess)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(context).Wrap(context);
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

        _actionResultWrapperFactory.CreateFor(context).Wrap(context);
    }

    public virtual void OnResultExecuted(ResultExecutedContext context)
    {
        //no action
    }
}