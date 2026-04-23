using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Extensions;
using ADTOSharp.MultiTenancy;
using Microsoft.AspNetCore.Http;
using System;

namespace ADTOSharp.AspNetCore.MultiTenancy;

public class HttpCookieTenantResolveContributor : ITenantResolveContributor, ITransientDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMultiTenancyConfig _multiTenancyConfig;

    public HttpCookieTenantResolveContributor(
        IHttpContextAccessor httpContextAccessor,
        IMultiTenancyConfig multiTenancyConfig)
    {
        _httpContextAccessor = httpContextAccessor;
        _multiTenancyConfig = multiTenancyConfig;
    }

    public Guid? ResolveTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        var tenantIdValue = httpContext.Request.Cookies[_multiTenancyConfig.TenantIdResolveKey];
        if (tenantIdValue.IsNullOrEmpty())
        {
            return null;
        }

        return Guid.TryParse(tenantIdValue, out var tenantId) ? tenantId : (Guid?)null;
    }
}