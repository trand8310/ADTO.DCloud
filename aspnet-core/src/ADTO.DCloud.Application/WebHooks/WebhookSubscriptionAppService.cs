using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Localization;
using ADTOSharp.Webhooks;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.WebHooks.Dto;

namespace ADTO.DCloud.WebHooks
{
    /// <summary>
    /// WEB事件订阅与发布
    /// </summary>
    [ADTOSharpAuthorize(PermissionNames.Pages_Administration_WebhookSubscription)]
    public class WebhookSubscriptionAppService : DCloudAppServiceBase, IWebhookSubscriptionAppService
    {
        #region Fields
        private readonly IWebhookSubscriptionManager _webHookSubscriptionManager;
        private readonly IAppWebhookPublisher _appWebhookPublisher;
        private readonly IWebhookDefinitionManager _webhookDefinitionManager;
        private readonly ILocalizationContext _localizationContext;
        #endregion

        #region Ctor

        public WebhookSubscriptionAppService(
            IWebhookSubscriptionManager webHookSubscriptionManager,
            IAppWebhookPublisher appWebhookPublisher,
            IWebhookDefinitionManager webhookDefinitionManager,
            ILocalizationContext localizationContext
            )
        {
            _webHookSubscriptionManager = webHookSubscriptionManager;
            _appWebhookPublisher = appWebhookPublisher;
            _webhookDefinitionManager = webhookDefinitionManager;
            _localizationContext = localizationContext;
        }
        #endregion
        #region Methods
        public async Task<string> PublishTestWebhook()
        {
            await _appWebhookPublisher.PublishTestWebhook();
            return L("WebhookSendAttemptInQueue") + "(" + L("YouHaveToSubscribeToTestWebhookToReceiveTestEvent") + ")";
        }

        public async Task<ListResultDto<GetAllSubscriptionsOutput>> GetAllSubscriptions()
        {
            var subscriptions = await _webHookSubscriptionManager.GetAllSubscriptionsAsync(ADTOSharpSession.TenantId);
            return new ListResultDto<GetAllSubscriptionsOutput>(
                ObjectMapper.Map<List<GetAllSubscriptionsOutput>>(subscriptions)
                );
        }

        [ADTOSharpAuthorize(
            PermissionNames.Pages_Administration_WebhookSubscription_Create,
            PermissionNames.Pages_Administration_WebhookSubscription_Edit,
            PermissionNames.Pages_Administration_WebhookSubscription_Detail
            )
        ]
        public async Task<WebhookSubscription> GetSubscription(string subscriptionId)
        {
            return await _webHookSubscriptionManager.GetAsync(Guid.Parse(subscriptionId));
        }

        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_WebhookSubscription_Create)]
        public async Task AddSubscription(WebhookSubscription subscription)
        {
            subscription.TenantId = ADTOSharpSession.TenantId;

            await _webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);
        }

        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_WebhookSubscription_Edit)]
        public async Task UpdateSubscription(WebhookSubscription subscription)
        {
            if (subscription.Id == default)
            {
                throw new ArgumentNullException(nameof(subscription.Id));
            }

            subscription.TenantId = ADTOSharpSession.TenantId;

            await _webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);
        }

        [ADTOSharpAuthorize(PermissionNames.Pages_Administration_WebhookSubscription_ChangeActivity)]
        public async Task ActivateWebhookSubscription(ActivateWebhookSubscriptionInput input)
        {
            await _webHookSubscriptionManager.ActivateWebhookSubscriptionAsync(input.SubscriptionId, input.IsActive);
        }

        public async Task<bool> IsSubscribed(string webhookName)
        {
            return await _webHookSubscriptionManager.IsSubscribedAsync(ADTOSharpSession.TenantId, webhookName);
        }

        public async Task<ListResultDto<GetAllSubscriptionsOutput>> GetAllSubscriptionsIfFeaturesGranted(string webhookName)
        {
            var subscriptions = await _webHookSubscriptionManager.GetAllSubscriptionsIfFeaturesGrantedAsync(ADTOSharpSession.TenantId, webhookName);
            return new ListResultDto<GetAllSubscriptionsOutput>(
                ObjectMapper.Map<List<GetAllSubscriptionsOutput>>(subscriptions)
            );
        }

        public async Task<ListResultDto<GetAllAvailableWebhooksOutput>> GetAllAvailableWebhooks()
        {
            var webhooks = _webhookDefinitionManager.GetAll();
            var definitions = new List<GetAllAvailableWebhooksOutput>();

            foreach (var webhookDefinition in webhooks)
            {
                if (await _webhookDefinitionManager.IsAvailableAsync(ADTOSharpSession.TenantId, webhookDefinition.Name))
                {
                    definitions.Add(new GetAllAvailableWebhooksOutput()
                    {
                        Name = webhookDefinition.Name,
                        Description = webhookDefinition.Description?.Localize(_localizationContext),
                        DisplayName = webhookDefinition.DisplayName?.Localize(_localizationContext)
                    });
                }
            }

            return new ListResultDto<GetAllAvailableWebhooksOutput>(definitions.OrderBy(d => d.Name).ToList());
        }
        #endregion
    }
}

