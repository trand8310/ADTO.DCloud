using System.Threading.Tasks;
using ADTO.DCloud.Sessions;
using ADTO.DCloud.Sessions.Dto;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Http;


namespace ADTO.DCloud.Web.Session;

public class PerRequestSessionCache : IPerRequestSessionCache, ITransientDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISessionAppService _sessionAppService;

    public PerRequestSessionCache(
        IHttpContextAccessor httpContextAccessor,
        ISessionAppService sessionAppService)
    {
        _httpContextAccessor = httpContextAccessor;
        _sessionAppService = sessionAppService;
    }

    public async Task<GetCurrentSessionOutput> GetCurrentSessionAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return await _sessionAppService.GetCurrentSessionAsync();
        }

        var cachedValue = httpContext.Items["__PerRequestSessionCache"] as GetCurrentSessionOutput;
        if (cachedValue == null)
        {
            cachedValue = await _sessionAppService.GetCurrentSessionAsync();
            httpContext.Items["__PerRequestSessionCache"] = cachedValue;
        }

        return cachedValue;
    }
}