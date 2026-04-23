using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Linq;

namespace ADTOSharp.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookSubscriptionsStore"/> using repositories.
    /// </summary>
    public class WebhookSubscriptionsStore : IWebhookSubscriptionsStore, ITransientDependency
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<WebhookSubscriptionInfo, Guid> _webhookSubscriptionRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookSubscriptionsStore(
            IRepository<WebhookSubscriptionInfo, Guid> webhookSubscriptionRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookSubscriptionRepository = webhookSubscriptionRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task<WebhookSubscriptionInfo> GetAsync(Guid id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _webhookSubscriptionRepository.GetAsync(id));
        }

        public virtual WebhookSubscriptionInfo Get(Guid id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Get(id));
        }

        public virtual async Task InsertAsync(WebhookSubscriptionInfo webhookInfo)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _webhookSubscriptionRepository.InsertAsync(webhookInfo);
            });
        }

        public virtual void Insert(WebhookSubscriptionInfo webhookInfo)
        {
            _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Insert(webhookInfo));
        }

        public virtual async Task UpdateAsync(WebhookSubscriptionInfo webhookSubscription)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _webhookSubscriptionRepository.UpdateAsync(webhookSubscription);
            });
        }

        public virtual void Update(WebhookSubscriptionInfo webhookSubscription)
        {
            _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Update(webhookSubscription));
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await _webhookSubscriptionRepository.DeleteAsync(id);
            });
        }

        public virtual void Delete(Guid id)
        {
            _unitOfWorkManager.WithUnitOfWork(() => _webhookSubscriptionRepository.Delete(id));
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(Guid? tenantId)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    subscriptionInfo.TenantId == tenantId);
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptions(Guid? tenantId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    subscriptionInfo.TenantId == tenantId);
            });
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsAsync(
            Guid? tenantId,
            string webhookName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    subscriptionInfo.TenantId == tenantId &&
                    subscriptionInfo.IsActive &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptions(Guid? tenantId, string webhookName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    subscriptionInfo.TenantId == tenantId &&
                    subscriptionInfo.IsActive &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfTenantsAsync(Guid?[] tenantIds)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    tenantIds.Contains(subscriptionInfo.TenantId)
                );
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptionsOfTenants(Guid?[] tenantIds)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    tenantIds.Contains(subscriptionInfo.TenantId)
                );
            });
        }

        public virtual async Task<List<WebhookSubscriptionInfo>> GetAllSubscriptionsOfTenantsAsync(
            Guid?[] tenantIds,
            string webhookName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _webhookSubscriptionRepository.GetAllListAsync(subscriptionInfo =>
                    subscriptionInfo.IsActive &&
                    tenantIds.Contains(subscriptionInfo.TenantId) &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual List<WebhookSubscriptionInfo> GetAllSubscriptionsOfTenants(Guid?[] tenantIds, string webhookName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAllList(subscriptionInfo =>
                    subscriptionInfo.IsActive &&
                    tenantIds.Contains(subscriptionInfo.TenantId) &&
                    subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                );
            });
        }

        public virtual async Task<bool> IsSubscribedAsync(Guid? tenantId, string webhookName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await AsyncQueryableExecuter.AnyAsync((await _webhookSubscriptionRepository.GetAllAsync())
                    .Where(subscriptionInfo =>
                        subscriptionInfo.TenantId == tenantId &&
                        subscriptionInfo.IsActive &&
                        subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                    ));
            });
        }

        public virtual bool IsSubscribed(Guid? tenantId, string webhookName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _webhookSubscriptionRepository.GetAll()
                    .Any(subscriptionInfo =>
                        subscriptionInfo.TenantId == tenantId &&
                        subscriptionInfo.IsActive &&
                        subscriptionInfo.Webhooks.Contains("\"" + webhookName + "\"")
                    );
            });
        }
    }
}
