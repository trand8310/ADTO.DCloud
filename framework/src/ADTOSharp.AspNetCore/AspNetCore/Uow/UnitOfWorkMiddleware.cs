using System.Threading.Tasks;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ADTOSharp.AspNetCore.Uow;

public class ADTOSharpUnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly UnitOfWorkMiddlewareOptions _options;

    public ADTOSharpUnitOfWorkMiddleware(
        RequestDelegate next,
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<UnitOfWorkMiddlewareOptions> options)
    {
        _next = next;
        _unitOfWorkManager = unitOfWorkManager;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (!_options.Filter(httpContext))
        {
            await _next(httpContext);
            return;
        }

        using (var uow = _unitOfWorkManager.Begin(_options.OptionsFactory(httpContext)))
        {
            await _next(httpContext);
            await uow.CompleteAsync();
        }
    }
}