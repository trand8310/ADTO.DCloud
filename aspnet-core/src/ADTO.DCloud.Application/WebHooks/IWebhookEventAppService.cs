using System.Threading.Tasks;
using ADTOSharp.Webhooks;

namespace ADTO.DCloud.WebHooks;

public interface IWebhookEventAppService
{
    Task<WebhookEvent> Get(string id);
}
