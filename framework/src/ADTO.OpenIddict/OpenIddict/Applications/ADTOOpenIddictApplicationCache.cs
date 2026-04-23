
using ADTOSharp;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Caching;
using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ADTO.OpenIddict.Applications;

public class ADTOOpenIddictApplicationCache : ADTOOpenIddictCacheBase<OpenIddictApplication, OpenIddictApplicationModel, IOpenIddictApplicationStore<OpenIddictApplicationModel>>,
    IOpenIddictApplicationCache<OpenIddictApplicationModel>,
    ITransientDependency
{
    public ADTOOpenIddictApplicationCache(
        ITypedCache<string, OpenIddictApplicationModel> cache,
        ITypedCache<string, OpenIddictApplicationModel[]> arrayCache,
        IOpenIddictApplicationStore<OpenIddictApplicationModel> store)
        : base(cache, arrayCache, store)
    {
    }

    public virtual async ValueTask AddAsync(OpenIddictApplicationModel application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        await RemoveAsync(application, cancellationToken);

        var data = new KeyValuePair<string, OpenIddictApplicationModel>[]
        {
            new KeyValuePair<string, OpenIddictApplicationModel>($"{nameof(FindByClientIdAsync)}_{await Store.GetClientIdAsync(application, cancellationToken)}", application),
            new KeyValuePair<string, OpenIddictApplicationModel>($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(application, cancellationToken)}", application)
        };
        await Cache.SetAsync(data);
    }


    public virtual async ValueTask<OpenIddictApplicationModel> FindByClientIdAsync(string clientId, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(clientId, nameof(clientId));

        return await Cache.GetAsync($"{nameof(FindByClientIdAsync)}_{clientId}", async () =>
        {
            var application = await Store.FindByClientIdAsync(clientId, cancellationToken);
            if (application != null)
            {
                await AddAsync(application, cancellationToken);
            }
            return application;
        });
    }

    public virtual async ValueTask<OpenIddictApplicationModel> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(id, nameof(id));

        return await Cache.GetAsync($"{nameof(FindByIdAsync)}_{id}", async () =>
        {
            var application = await Store.FindByIdAsync(id, cancellationToken);
            if (application != null)
            {
                await AddAsync(application, cancellationToken);
            }
            return application;
        });
    }

    public virtual async IAsyncEnumerable<OpenIddictApplicationModel> FindByPostLogoutRedirectUriAsync(string address, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(address, nameof(address));

        var applications = await ArrayCache.GetAsync($"{nameof(FindByPostLogoutRedirectUriAsync)}_{address}", async () =>
        {
            var applications = new List<OpenIddictApplicationModel>();
            await foreach (var application in Store.FindByPostLogoutRedirectUriAsync(address, cancellationToken))
            {
                applications.Add(application);
                await AddAsync(application, cancellationToken);
            }
            return applications.ToArray();

        });

        foreach (var application in applications)
        {
            yield return application;
        }
    }

    public virtual async IAsyncEnumerable<OpenIddictApplicationModel> FindByRedirectUriAsync(string address, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(address, nameof(address));

        var applications = await ArrayCache.GetAsync($"{nameof(FindByRedirectUriAsync)}_{address}", async () =>
        {
            var applications = new List<OpenIddictApplicationModel>();
            await foreach (var application in Store.FindByRedirectUriAsync(address, cancellationToken))
            {
                applications.Add(application);
                await AddAsync(application, cancellationToken);
            }
            return applications.ToArray();

        });

        foreach (var application in applications)
        {
            yield return application;
        }
    }

    public virtual async ValueTask RemoveAsync(OpenIddictApplicationModel application, CancellationToken cancellationToken)
    {
        Check.NotNull(application, nameof(application));

        await Cache.RemoveAsync($"{nameof(FindByClientIdAsync)}_{await Store.GetClientIdAsync(application, cancellationToken)}");
        await Cache.RemoveAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(application, cancellationToken)}");

        var redirectUris = await Store.GetRedirectUrisAsync(application, cancellationToken);
        await ArrayCache.RemoveAsync(redirectUris.Select(address => $"{nameof(FindByRedirectUriAsync)}_{address}").ToArray());

        var postLogoutRedirectUris = await Store.GetPostLogoutRedirectUrisAsync(application, cancellationToken);
        await ArrayCache.RemoveAsync(postLogoutRedirectUris.Select(address => $"{nameof(FindByPostLogoutRedirectUriAsync)}_{address}").ToArray());
    }
}
