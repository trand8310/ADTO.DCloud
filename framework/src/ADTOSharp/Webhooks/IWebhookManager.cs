using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ADTOSharp.Webhooks
{
    public interface IWebhookManager
    {
        Task<WebhookPayload> GetWebhookPayloadAsync(WebhookSenderArgs webhookSenderArgs);

        WebhookPayload GetWebhookPayload(WebhookSenderArgs webhookSenderArgs);

        void SignWebhookRequest(HttpRequestMessage request, string serializedBody, string secret);
        
        string GetSerializedBody(WebhookSenderArgs webhookSenderArgs);

        Task<string> GetSerializedBodyAsync(WebhookSenderArgs webhookSenderArgs);

        Task<Guid> InsertAndGetIdWebhookSendAttemptAsync(WebhookSenderArgs webhookSenderArgs);

        Task StoreResponseOnWebhookSendAttemptAsync(
            Guid webhookSendAttemptId, Guid? tenantId,
            HttpStatusCode? statusCode, string content);
    }
}
