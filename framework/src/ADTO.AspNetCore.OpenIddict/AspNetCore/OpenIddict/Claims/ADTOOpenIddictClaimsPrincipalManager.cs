using System.Security.Claims;
using System.Threading.Tasks;
using ADTOSharp.Dependency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace ADTO.AspNetCore.OpenIddict.Claims;

public class ADTOOpenIddictClaimsPrincipalManager : ISingletonDependency
{
    protected IServiceScopeFactory ServiceScopeFactory { get; }
    protected IOptions<ADTOOpenIddictClaimsPrincipalOptions> Options { get; }

    public ADTOOpenIddictClaimsPrincipalManager(IServiceScopeFactory serviceScopeFactory, IOptions<ADTOOpenIddictClaimsPrincipalOptions> options)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Options = options;
    }

    public virtual async Task HandleAsync(OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            foreach (var providerType in Options.Value.ClaimsPrincipalHandlers)
            {
                var provider = (IADTOOpenIddictClaimsPrincipalHandler)scope.ServiceProvider.GetRequiredService(providerType);
                await provider.HandleAsync(new ADTOOpenIddictClaimsPrincipalHandlerContext(scope.ServiceProvider, openIddictRequest, principal));
            }
        }
    }
}