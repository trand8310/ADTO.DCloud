using System;
using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Mvc.Extensions;
using ADTOSharp.AspNetCore.Mvc.Results;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Events.Bus;
using ADTOSharp.Events.Bus.Exceptions;
using ADTOSharp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ADTOSharp.AspNetCore.Mvc.Authorization;

public class ADTOSharpAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
{
    public ILogger Logger { get; set; }

    private readonly IAuthorizationHelper _authorizationHelper;
    private readonly IErrorInfoBuilder _errorInfoBuilder;
    private readonly IEventBus _eventBus;

    public ADTOSharpAuthorizationFilter(
        IAuthorizationHelper authorizationHelper,
        IErrorInfoBuilder errorInfoBuilder,
        IEventBus eventBus)
    {
        _authorizationHelper = authorizationHelper;
        _errorInfoBuilder = errorInfoBuilder;
        _eventBus = eventBus;
        Logger = NullLogger.Instance;
    }

    public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context?.HttpContext?.GetEndpoint();
        // Allow Anonymous skips all authorization
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            return;
        }

        if (!context.ActionDescriptor.IsControllerAction())
        {
            return;
        }

        //TODO: Avoid using try/catch, use conditional checking
        try
        {
            await _authorizationHelper.AuthorizeAsync(
                context.ActionDescriptor.GetMethodInfo(),
                context.ActionDescriptor.GetMethodInfo().DeclaringType
            );
        }
        catch (ADTOSharpAuthorizationException ex)
        {
            Logger.Warn(ex.ToString(), ex);

            await _eventBus.TriggerAsync(this, new ADTOSharpHandledExceptionData(ex));

            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

            if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                context.Result = new ObjectResult(new AjaxResponse(_errorInfoBuilder.BuildForException(ex), true))
                {
                    StatusCode = isAuthenticated
                        ? (int)System.Net.HttpStatusCode.Forbidden
                        : (int)System.Net.HttpStatusCode.Unauthorized
                };
            }
            else
            {
                if (isAuthenticated)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    context.Result = new ChallengeResult();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString(), ex);

            await _eventBus.TriggerAsync(this, new ADTOSharpHandledExceptionData(ex));

            if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                context.Result = new ObjectResult(new AjaxResponse(_errorInfoBuilder.BuildForException(ex)))
                {
                    StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                };
            }
            else
            {
                //TODO: How to return Error page?
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}