using ADTOSharp.AspNetCore.Middleware;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Events.Bus;
using ADTOSharp.Events.Bus.Exceptions;
using ADTOSharp.Json;
using ADTOSharp.Localization;
using ADTOSharp.Web;
using ADTOSharp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;
using System.Threading.Tasks;


namespace ADTOSharp.AspNetCore.ExceptionHandling;

public class ADTOSharpAuthorizationExceptionHandlingMiddleware : ADTOSharpMiddlewareBase, ITransientDependency
{
    private readonly IErrorInfoBuilder _errorInfoBuilder;
    private readonly ILocalizationManager _localizationManager;
    private readonly Func<object, Task> _clearCacheHeadersDelegate;


    public ILogger Logger { get; set; }

    public IEventBus EventBus { get; set; }

    public ADTOSharpAuthorizationExceptionHandlingMiddleware(
        IErrorInfoBuilder errorInfoBuilder,
        ILocalizationManager localizationManager)
    {
        _errorInfoBuilder = errorInfoBuilder;
        _localizationManager = localizationManager;

        EventBus = NullEventBus.Instance;
        Logger = NullLogger.Instance;

        _clearCacheHeadersDelegate = ClearCacheHeaders;
    }



    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (IsAuthorizationExceptionStatusCode(context))
        {
            var exception = new ADTOSharpAuthorizationException(GetAuthorizationExceptionMessage(context));

            Logger.Error(exception.Message);

            await context.Response.WriteAsync(
                new AjaxResponse(
                    _errorInfoBuilder.BuildForException(exception),
                    true
                ).ToJsonString()
            );

            await EventBus.TriggerAsync(this, new ADTOSharpHandledExceptionData(exception));
        }
    }


    //private async Task HandleAndWrapException(HttpContext httpContext, Exception exception)
    //{
    //    var exceptionHandlingOptions = httpContext.RequestServices.GetRequiredService<IOptions<ADTOSharpExceptionHandlingOptions>>().Value;

    //    if (exceptionHandlingOptions.ShouldLogException(exception))
    //    {
    //        //_logger.LogException(exception);
    //    }

    //    await httpContext
    //        .RequestServices
    //        .GetRequiredService<IExceptionNotifier>()
    //        .NotifyAsync(
    //            new ExceptionNotificationContext(exception)
    //        );

    //    if (exception is ADTOSharpAuthorizationException)
    //    {
    //        await httpContext.RequestServices.GetRequiredService<IADTOSharpAuthorizationExceptionHandler>()
    //            .HandleAsync(exception.As<ADTOSharpAuthorizationException>(), httpContext);
    //    }
    //    else
    //    {
    //        var errorInfoConverter = httpContext.RequestServices.GetRequiredService<IExceptionToErrorInfoConverter>();
    //        var statusCodeFinder = httpContext.RequestServices.GetRequiredService<IHttpExceptionStatusCodeFinder>();
    //        var jsonSerializer = httpContext.RequestServices.GetRequiredService<IJsonSerializer>();

    //        httpContext.Response.Clear();
    //        httpContext.Response.StatusCode = (int)statusCodeFinder.GetStatusCode(httpContext, exception);
    //        httpContext.Response.OnStarting(_clearCacheHeadersDelegate, httpContext.Response);
    //        //httpContext.Response.Headers.Append(ADTOSharpHttpConsts.AbpErrorFormat, "true");
    //        httpContext.Response.Headers.Append("Content-Type", "application/json");

    //        await httpContext.Response.WriteAsync(
    //            jsonSerializer.Serialize(
    //                new RemoteServiceErrorResponse(
    //                    errorInfoConverter.Convert(exception, options =>
    //                    {
    //                        options.SendExceptionsDetailsToClients = exceptionHandlingOptions.SendExceptionsDetailsToClients;
    //                        options.SendStackTraceToClients = exceptionHandlingOptions.SendStackTraceToClients;
    //                        options.SendExceptionDataToClientTypes = exceptionHandlingOptions.SendExceptionDataToClientTypes;
    //                    })
    //                )
    //            )
    //        );
    //    }
    //}


    protected virtual string GetAuthorizationExceptionMessage(HttpContext context)
    {
        if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
        {
            _localizationManager.GetString(ADTOSharpWebConsts.LocalizationSourceName, "DefaultError403");
        }

        return _localizationManager.GetString(ADTOSharpWebConsts.LocalizationSourceName, "DefaultError401");
    }

    protected virtual bool IsAuthorizationExceptionStatusCode(HttpContext context)
    {
        return context.Response.StatusCode == (int)HttpStatusCode.Forbidden
               || context.Response.StatusCode == (int)HttpStatusCode.Unauthorized;
    }


    private Task ClearCacheHeaders(object state)
    {
        var response = (HttpResponse)state;

        response.Headers[HeaderNames.CacheControl] = "no-cache";
        response.Headers[HeaderNames.Pragma] = "no-cache";
        response.Headers[HeaderNames.Expires] = "-1";
        response.Headers.Remove(HeaderNames.ETag);

        return Task.CompletedTask;
    }
}