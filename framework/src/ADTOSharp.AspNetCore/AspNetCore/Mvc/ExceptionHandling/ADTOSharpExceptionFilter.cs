using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.AspNetCore.ExceptionHandling;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.AspNetCore.Mvc.Results;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Events.Bus;
using ADTOSharp.Events.Bus.Exceptions;
using ADTOSharp.ExceptionHandling;
using ADTOSharp.Extensions;
using ADTOSharp.Http;
using ADTOSharp.Logging;
using ADTOSharp.Reflection;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.Web.Configuration;
using ADTOSharp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ExceptionHandling;
using NullLogger = Castle.Core.Logging.NullLogger;


namespace ADTOSharp.AspNetCore.Mvc.ExceptionHandling;

public class ADTOSharpExceptionFilter : IAsyncExceptionFilter, ITransientDependency
{
    public Castle.Core.Logging.ILogger Logger { get; set; }

    public IEventBus EventBus { get; set; }

    private readonly IErrorInfoBuilder _errorInfoBuilder;
    private readonly IADTOSharpAspNetCoreConfiguration _configuration;
    private readonly IADTOSharpWebCommonModuleConfiguration _adtoWebCommonModuleConfiguration;

    public ADTOSharpExceptionFilter(
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

    protected virtual bool ShouldHandleException(ExceptionContext context)
    {
        //TODO: Create DontWrap attribute to control wrapping..?

        if (context.ExceptionHandled)
        {
            return false;
        }

        if (context.ActionDescriptor.IsControllerAction() &&
            context.ActionDescriptor.HasObjectResult())
        {
            return true;
        }

        if (context.HttpContext.Request.CanAccept(ADTOSharp.Http.MimeTypes.Application.Json))
        {
            return true;
        }

        if (context.HttpContext.Request.IsAjax())
        {
            return true;
        }

        return false;
    }


    public virtual async Task OnExceptionAsync(ExceptionContext context)
    {
        if (!context.ActionDescriptor.IsControllerAction())
        {
            return;
        }

        var wrapResultAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
            context.ActionDescriptor.GetMethodInfo(),
            _configuration.DefaultWrapResultAttribute
        );

        if (wrapResultAttribute.LogError)
        {
            LogHelper.LogException(Logger, context.Exception);
        }

        HandleAndWrapException(context, wrapResultAttribute);
        await Task.CompletedTask;
    }

    protected virtual void HandleAndWrapException(ExceptionContext context, WrapResultAttribute wrapResultAttribute)
    {
        if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
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

    private void HandleError(ExceptionContext context)
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

    protected virtual int GetStatusCode(ExceptionContext context, bool wrapOnError)
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