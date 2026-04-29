
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ADTO.DCloud.ScheduleTasks;

/// <summary>
/// Represents the HTTP client to request current store
/// </summary>
public partial class StoreHttpClient
{
    #region Fields

    protected readonly HttpClient _httpClient;

    #endregion

    #region Ctor

    public StoreHttpClient(HttpClient client)
    {
        //configure client
        client.BaseAddress = new Uri("http://localhost:44380");
        _httpClient = client;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Keep the current store site alive
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the asynchronous task whose result determines that request completed
    /// </returns>
    public virtual async Task KeepAliveAsync()
    {
        await _httpClient.GetStringAsync(AppConsts.KeepAlivePath);
    }

    #endregion
}