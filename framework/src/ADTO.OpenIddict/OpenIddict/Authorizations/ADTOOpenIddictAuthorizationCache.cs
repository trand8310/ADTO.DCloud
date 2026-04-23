using ADTO.OpenIddict.Applications;
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching;
using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


namespace ADTO.OpenIddict.Authorizations;

public class ADTOOpenIddictAuthorizationCache : ADTOOpenIddictCacheBase<OpenIddictAuthorization, OpenIddictAuthorizationModel, IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel>>,
    IOpenIddictAuthorizationCache<OpenIddictAuthorizationModel>,
    ITransientDependency
{
    public ADTOOpenIddictAuthorizationCache(
        ITypedCache<string, OpenIddictAuthorizationModel> cache,
        ITypedCache<string, OpenIddictAuthorizationModel[]> arrayCache,
        IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel> store)
        : base(cache, arrayCache, store)
    {
    }

    public virtual async ValueTask AddAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        await RemoveAsync(authorization, cancellationToken);

        await Cache.SetAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(authorization, cancellationToken)}", authorization);
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, string type, ImmutableArray<string>? scopes, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Note: this method is only partially cached.
        await foreach (var authorization in Store.FindAsync(subject, client, status, type, scopes, cancellationToken))
        {
            await AddAsync(authorization, cancellationToken);
            yield return authorization;
        }
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindByApplicationIdAsync(string applicationId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(applicationId, nameof(applicationId));

        var authorizations = await ArrayCache.GetAsync($"{nameof(FindByApplicationIdAsync)}_{applicationId}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindByApplicationIdAsync(applicationId, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        });

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    public virtual async ValueTask<OpenIddictAuthorizationModel> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(id, nameof(id));

        return await Cache.GetAsync($"{nameof(FindByIdAsync)}_{id}",
            async () => await Store.FindByIdAsync(id, cancellationToken));
    }

    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindBySubjectAsync(string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));

        var authorizations = await ArrayCache.GetAsync($"{nameof(FindBySubjectAsync)}_{subject}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindBySubjectAsync(subject, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        });

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    public virtual async ValueTask RemoveAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        await ArrayCache.RemoveAsync(new[]
        {
            $"{nameof(FindAsync)}_{await Store.GetSubjectAsync(authorization, cancellationToken)}_{await Store.GetApplicationIdAsync(authorization, cancellationToken)}_{await Store.GetStatusAsync(authorization, cancellationToken)}_{await Store.GetTypeAsync(authorization, cancellationToken)}",
            $"{nameof(FindByApplicationIdAsync)}_{await Store.GetApplicationIdAsync(authorization, cancellationToken)}",
            $"{nameof(FindBySubjectAsync)}_{await Store.GetSubjectAsync(authorization, cancellationToken)}"
        });

        await Cache.RemoveAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(authorization, cancellationToken)}");
    }
}
