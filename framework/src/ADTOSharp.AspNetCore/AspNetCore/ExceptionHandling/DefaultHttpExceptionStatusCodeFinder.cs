using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Uow;
using ADTOSharp.ExceptionHandling;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Net;



namespace ADTOSharp.AspNetCore.ExceptionHandling;

public class DefaultHttpExceptionStatusCodeFinder : IHttpExceptionStatusCodeFinder, ITransientDependency
{
    protected ADTOSharpExceptionHttpStatusCodeOptions Options { get; }

    public DefaultHttpExceptionStatusCodeFinder(
        IOptions<ADTOSharpExceptionHttpStatusCodeOptions> options)
    {
        Options = options.Value;
    }

    public virtual HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception)
    {
        if (exception is IHasHttpStatusCode exceptionWithHttpStatusCode &&
            exceptionWithHttpStatusCode.HttpStatusCode > 0)
        {
            return (HttpStatusCode)exceptionWithHttpStatusCode.HttpStatusCode;
        }

        if (exception is IHasErrorCode exceptionWithErrorCode &&
            !exceptionWithErrorCode.Code.IsNullOrWhiteSpace())
        {
            if (Options.ErrorCodeToHttpStatusCodeMappings.TryGetValue(exceptionWithErrorCode.Code!, out var status))
            {
                return status;
            }
        }

        if (exception is ADTOSharpAuthorizationException)
        {
            return httpContext.User.Identity!.IsAuthenticated
                ? HttpStatusCode.Forbidden
                : HttpStatusCode.Unauthorized;
        }

        //TODO: Handle SecurityException..?

        if (exception is ADTOSharpValidationException)
        {
            return HttpStatusCode.BadRequest;
        }

        if (exception is EntityNotFoundException)
        {
            return HttpStatusCode.NotFound;
        }

        if (exception is ADTOSharpDbConcurrencyException)
        {
            return HttpStatusCode.Conflict;
        }

        if (exception is NotImplementedException)
        {
            return HttpStatusCode.NotImplemented;
        }

        if (exception is IBusinessException)
        {
            return HttpStatusCode.Forbidden;
        }

        return HttpStatusCode.InternalServerError;
    }
}
