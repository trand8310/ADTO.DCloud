using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace ADTO.OpenIddict.Authorizations;

public class ADTOAuthorizationManager : OpenIddictAuthorizationManager<OpenIddictAuthorizationModel>
{
    protected ADTOOpenIddictIdentifierConverter IdentifierConverter { get; }

    public ADTOAuthorizationManager(
        [NotNull] [ItemNotNull] IOpenIddictAuthorizationCache<OpenIddictAuthorizationModel> cache,
        [NotNull] [ItemNotNull] ILogger<OpenIddictAuthorizationManager<OpenIddictAuthorizationModel>> logger,
        [NotNull] [ItemNotNull] IOptionsMonitor<OpenIddictCoreOptions> options,
        [NotNull] IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel> resolver,
        ADTOOpenIddictIdentifierConverter identifierConverter)
        : base(cache, logger, options, resolver)
    {
        IdentifierConverter = identifierConverter;
    }

    public async override ValueTask UpdateAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken = default)
    {
        if (!Options.CurrentValue.DisableEntityCaching)
        {
            var entity = await Store.FindByIdAsync(IdentifierConverter.ToString(authorization.Id), cancellationToken);
            if (entity != null)
            {
                await Cache.RemoveAsync(entity, cancellationToken);
            }
        }

        await base.UpdateAsync(authorization, cancellationToken);
    }
}
