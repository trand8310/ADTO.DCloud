using System.Net;
using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.Mvc.Results;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Events.Bus;
using ADTOSharp.Events.Bus.Exceptions;
using ADTOSharp.Logging;
using ADTOSharp.Reflection;
using ADTOSharp.Runtime;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.Web.Configuration;
using ADTOSharp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.ExceptionHandling;

public class ADTOSharpExceptionPageFilter : IAsyncPageFilter, ITransientDependency
{
    public ILogger Logger { get; set; }

    public IEventBus EventBus { get; set; }

    private readonly IErrorInfoBuilder _errorInfoBuilder;
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;
    private readonly IADTOSharpWebCommonModuleConfiguration _adtoWebCommonModuleConfiguration;

    public ADTOSharpExceptionPageFilter(
        IErrorInfoBuilder errorInfoBuilder,
        IADTOSharpAspNetCoreConfiguration configuration,
        IADTOSharpWebCommonModuleConfiguration adtoWebCommonModuleConfiguration)
    {
        _errorInfoBuilder = errorInfoBuilder;
        _configuration = configuration;
        _adtoWebCommonModuleConfiguration = adtoWebCommonModuleConfiguration;

        Logger = NullLogger.Instance;
        EventBus = NullEventBus.Instance;
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next)
    {
        if (context.HandlerMethod == null)
        {
            await next();
            return;
        }

        var pageHandlerExecutedContext = await next();

        if (pageHandlerExecutedContext.Exception == null)
        {
            return;
        }

        var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
            context.HandlerMethod.MethodInfo,
            _configuration.DefaultWrapResultAttribute
        );

        if (wrapResultAttribute.LogError)
        {
            LogHelper.LogException(Logger, pageHandlerExecutedContext.Exception);
        }

        HandleAndWrapException(pageHandlerExecutedContext, wrapResultAttribute);
    }

    protected virtual void HandleAndWrapException(PageHandlerExecutedContext context,
        WrapResultAttribute wrapResultAttribute)
    {
        if (!ActionResultHelper.IsObjectResult(context.HandlerMethod.MethodInfo.ReturnType))
        {
            return;
        }

        var displayUrl = context.HttpContext.Request.GetDisplayUrl();
        if (_adtoWebCommonModuleConfiguration.WrapResultFilters.HasFilterForWrapOnError(displayUrl,
            out var wrapOnError))
        {
            context.HttpContext.Response.StatusCode = GetStatusCode(context, wrapOnError);

            if (!wrapOnError)
            {
                return;
            }

            HandleError(context);
            return;
        }

        context.HttpContext.Response.StatusCode = GetStatusCode(context, wrapResultAttribute.WrapOnError);

        if (!wrapResultAttribute.WrapOnError)
        {
            return;
        }

        HandleError(context);
    }

    private void HandleError(PageHandlerExecutedContext context)
    {
        context.Result = new ObjectResult(
            new AjaxResponse(
                _errorInfoBuilder.BuildForException(context.Exception),
                context.Exception is ADTOSharpAuthorizationException
            )
        );

        EventBus.Trigger(this, new ADTOSharpHandledExceptionData(context.Exception));

        context.Exception = null; // Handled!
    }

    protected virtual int GetStatusCode(PageHandlerExecutedContext context, bool wrapOnError)
    {
        if (context.Exception is ADTOSharpAuthorizationException)
        {
            return context.HttpContext.User.Identity.IsAuthenticated
                ? (int)HttpStatusCode.Forbidden
                : (int)HttpStatusCode.Unauthorized;
        }

        if (context.Exception is ADTOSharpValidationException)
        {
            return (int)HttpStatusCode.BadRequest;
        }

        if (context.Exception is EntityNotFoundException)
        {
            return (int)HttpStatusCode.NotFound;
        }

        if (wrapOnError)
        {
            return (int)HttpStatusCode.InternalServerError;
        }

        return context.HttpContext.Response.StatusCode;
    }
}