using ADTO.OpenIddict.Authorizations;
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching;
using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ADTO.OpenIddict.Scopes;

public class ADTOOpenIddictScopeCache : ADTOOpenIddictCacheBase<OpenIddictScope, OpenIddictScopeModel, IOpenIddictScopeStore<OpenIddictScopeModel>>,
    IOpenIddictScopeCache<OpenIddictScopeModel>,
    ITransientDependency
{
    public ADTOOpenIddictScopeCache(
        ITypedCache<string, OpenIddictScopeModel> cache,
        ITypedCache<string, OpenIddictScopeModel[]> arrayCache,
        IOpenIddictScopeStore<OpenIddictScopeModel> store)
        : base(cache, arrayCache, store)
    {
    }

    public virtual async ValueTask AddAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        await RemoveAsync(scope, cancellationToken);

        await Cache.SetAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(scope, cancellationToken)}", scope);
        await Cache.SetAsync($"{nameof(FindByNameAsync)}_{await Store.GetNameAsync(scope, cancellationToken)}", scope);
    }

    public virtual async ValueTask<OpenIddictScopeModel> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(id, nameof(id));

        return await Cache.GetAsync($"{nameof(FindByIdAsync)}_{id}",  async () =>
        {
            var scope = await Store.FindByIdAsync(id, cancellationToken);
            if (scope != null)
            {
                await AddAsync(scope, cancellationToken);
            }
            return scope;
        });
    }

    public virtual async ValueTask<OpenIddictScopeModel> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(name, nameof(name));

        return await Cache.GetAsync($"{nameof(FindByNameAsync)}_{name}",  async () =>
        {
            var scope = await Store.FindByNameAsync(name, cancellationToken);
            if (scope != null)
            {
                await AddAsync(scope, cancellationToken);
            }
            return scope;
        });
    }

    public virtual async IAsyncEnumerable<OpenIddictScopeModel> FindByNamesAsync(ImmutableArray<string> names, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNull(names, nameof(names));

        foreach (var name in names)
        {
            Check.NotNullOrEmpty(name, nameof(name));
        }

        // Note: this method is only partially cached.
        await foreach (var scope in Store.FindByNamesAsync(names, cancellationToken))
        {
            await AddAsync(scope, cancellationToken);
            yield return scope;
        }
    }

    public virtual async IAsyncEnumerable<OpenIddictScopeModel> FindByResourceAsync(string resource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(resource, nameof(resource));

        var scopes = await ArrayCache.GetAsync($"{nameof(FindByResourceAsync)}_{resource}", async () =>
        {
            var scopes = new List<OpenIddictScopeModel>();
            await foreach (var scope in Store.FindByResourceAsync(resource, cancellationToken))
            {
                scopes.Add(scope);
                await AddAsync(scope, cancellationToken);
            }
            return scopes.ToArray();
        });

        foreach (var scope in scopes)
        {
            yield return scope;
        }
    }

    public virtual async ValueTask RemoveAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        var resources = new List<string>();
        foreach (var resource in await Store.GetResourcesAsync(scope, cancellationToken))
        {
            resources.Add($"{nameof(FindByResourceAsync)}_{resource}");
        }
        await ArrayCache.RemoveAsync(resources.ToArray());

        await Cache.RemoveAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(scope, cancellationToken)}");
        await Cache.RemoveAsync($"{nameof(FindByNameAsync)}_{await Store.GetNameAsync(scope, cancellationToken)}");
    }
}
