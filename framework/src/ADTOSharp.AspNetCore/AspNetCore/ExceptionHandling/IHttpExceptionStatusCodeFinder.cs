using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace ADTOSharp.AspNetCore.ExceptionHandling;

public interface IHttpExceptionStatusCodeFinder
{
    HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception);
}
