using System;
using System.Threading.Tasks;
using ADTOSharp.Configuration;
using Microsoft.AspNetCore.Http;

namespace ADTOSharp.AspNetCore.Mvc.Caching;

public class GetScriptsResponsePerUserCacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IGetScriptsResponsePerUserConfiguration _configuration;

    public GetScriptsResponsePerUserCacheMiddleware(RequestDelegate next,
        IGetScriptsResponsePerUserConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        if (_configuration.IsEnabled && context.Request.Path == "/ADTOSharpScripts/GetScripts")
        {
            context.Response.GetTypedHeaders().CacheControl =
                new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = _configuration.MaxAge,
                };
        }

        await _next(context);
    }
}