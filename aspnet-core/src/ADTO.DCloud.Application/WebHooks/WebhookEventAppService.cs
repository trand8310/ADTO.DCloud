using System;
using System.Threading.Tasks;
using ADTOSharp.Authorization;
using ADTOSharp.Webhooks;
using ADTO.DCloud.Authorization;

namespace ADTO.DCloud.WebHooks;

/// <summary>
/// 事件订阅服务
/// </summary>
[ADTOSharpAuthorize(PermissionNames.Pages_Administration_WebhookSubscription)]
public class WebhookEventAppService : DCloudAppServiceBase, IWebhookEventAppService
{
    private readonly IWebhookEventStore _webhookEventStore;

    public WebhookEventAppService(IWebhookEventStore webhookEventStore)
    {
        _webhookEventStore = webhookEventStore;
    }

    public async Task<WebhookEvent> Get(string id)
    {
        return await _webhookEventStore.GetAsync(ADTOSharpSession.TenantId, Guid.Parse(id));
    }
}

