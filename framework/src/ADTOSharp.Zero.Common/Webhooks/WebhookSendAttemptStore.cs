using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Linq;

namespace ADTOSharp.Webhooks
{
    /// <summary>
    /// Implements <see cref="IWebhookSendAttemptStore"/> using repositories.
    /// </summary>
    public class WebhookSendAttemptStore : IWebhookSendAttemptStore, ITransientDependency
    {
        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

        private readonly IRepository<WebhookSendAttempt, Guid> _webhookSendAttemptRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public WebhookSendAttemptStore(
            IRepository<WebhookSendAttempt, Guid> webhookSendAttemptRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _webhookSendAttemptRepository = webhookSendAttemptRepository;
            _unitOfWorkManager = unitOfWorkManager;

            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }

        public virtual async Task InsertAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
                {
                    await _webhookSendAttemptRepository.InsertAsync(webhookSendAttempt);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void Insert(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
                {
                    _webhookSendAttemptRepository.Insert(webhookSendAttempt);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task UpdateAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
                {
                    await _webhookSendAttemptRepository.UpdateAsync(webhookSendAttempt);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public virtual void Update(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
                {
                    _webhookSendAttemptRepository.Update(webhookSendAttempt);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task DeleteAsync(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
                {
                    await _webhookSendAttemptRepository.DeleteAsync(webhookSendAttempt);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }

                await uow.CompleteAsync();
            }
        }

        public void Delete(WebhookSendAttempt webhookSendAttempt)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(webhookSendAttempt.TenantId))
                {
                    _webhookSendAttemptRepository.Delete(webhookSendAttempt);
                    _unitOfWorkManager.Current.SaveChanges();
                }

                uow.Complete();
            }
        }

        public virtual async Task<WebhookSendAttempt> GetAsync(Guid? tenantId, Guid id)
        {
            WebhookSendAttempt sendAttempt;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    sendAttempt = await _webhookSendAttemptRepository.GetAsync(id);
                }

                await uow.CompleteAsync();
            }

            return sendAttempt;
        }

        public virtual WebhookSendAttempt Get(Guid? tenantId, Guid id)
        {
            WebhookSendAttempt sendAttempt;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    sendAttempt = _webhookSendAttemptRepository.Get(id);
                }

                uow.CompleteAsync();
            }

            return sendAttempt;
        }

        public virtual async Task<int> GetSendAttemptCountAsync(Guid? tenantId, Guid webhookEventId,
            Guid webhookSubscriptionId)
        {
            int sendAttemptCount;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    sendAttemptCount = await _webhookSendAttemptRepository
                        .CountAsync(attempt =>
                            attempt.WebhookEventId == webhookEventId &&
                            attempt.WebhookSubscriptionId == webhookSubscriptionId
                        );
                }

                await uow.CompleteAsync();
            }

            return sendAttemptCount;
        }

        public virtual int GetSendAttemptCount(Guid? tenantId, Guid webhookId, Guid webhookSubscriptionId)
        {
            int sendAttemptCount;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    sendAttemptCount = _webhookSendAttemptRepository.GetAll()
                        .Count(attempt =>
                            attempt.WebhookEventId == webhookId &&
                            attempt.WebhookSubscriptionId == webhookSubscriptionId);
                }

                uow.Complete();
            }

            return sendAttemptCount;
        }

        public virtual async Task<bool> HasXConsecutiveFailAsync(Guid? tenantId, Guid subscriptionId, int failCount)
        {
            bool result;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (await _webhookSendAttemptRepository.CountAsync(x => x.WebhookSubscriptionId == subscriptionId) <
                        failCount)
                    {
                        result = false;
                    }
                    else
                    {
                        result = !await AsyncQueryableExecuter.AnyAsync(
                            (await _webhookSendAttemptRepository.GetAllAsync())
                                .OrderByDescending(attempt => attempt.CreationTime)
                                .Take(failCount)
                                .Where(attempt => attempt.ResponseStatusCode == HttpStatusCode.OK)
                        );
                    }
                }

                await uow.CompleteAsync();
            }

            return result;
        }

        public virtual async Task<IPagedResult<WebhookSendAttempt>> GetAllSendAttemptsBySubscriptionAsPagedListAsync(
            Guid? tenantId,
            Guid subscriptionId,
            int pageNumber, 
            int pageSize)
        {
            PagedResultDto<WebhookSendAttempt> sendAttempts;
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var query = _webhookSendAttemptRepository.GetAllIncluding(attempt => attempt.WebhookEvent)
                        .Where(attempt =>
                            attempt.WebhookSubscriptionId == subscriptionId
                        );

                    var totalCount = await AsyncQueryableExecuter.CountAsync(query);

                    var list = await AsyncQueryableExecuter.ToListAsync(query
                        .OrderByDescending(attempt => attempt.CreationTime)
                        .Skip((pageNumber > 1 ? (pageNumber - 1) * pageSize : 0))
                        .Take(pageSize)
                    );

                    sendAttempts = new PagedResultDto<WebhookSendAttempt>
                    {
                        TotalCount = totalCount,
                        Items = list
                    };
                }

                await uow.CompleteAsync();
            }

            return sendAttempts;
        }

        public virtual IPagedResult<WebhookSendAttempt> GetAllSendAttemptsBySubscriptionAsPagedList(Guid? tenantId,
            Guid subscriptionId, int pageNumber,int pageSize)
        {
            PagedResultDto<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var query = _webhookSendAttemptRepository.GetAllIncluding(attempt => attempt.WebhookEvent)
                        .Where(attempt =>
                            attempt.WebhookSubscriptionId == subscriptionId
                        );

                    var totalCount = query.Count();

                    var list = query
                        .OrderByDescending(attempt => attempt.CreationTime)
                        .Skip((pageNumber > 1 ? (pageNumber - 1) * pageSize : 0))
                        .Take(pageSize)
                        .ToList();

                    sendAttempts = new PagedResultDto<WebhookSendAttempt>()
                    {
                        TotalCount = totalCount,
                        Items = list
                    };
                }

                uow.Complete();
            }

            return sendAttempts;
        }

        public virtual async Task<List<WebhookSendAttempt>> GetAllSendAttemptsByWebhookEventIdAsync(Guid? tenantId,
            Guid webhookEventId)
        {
            List<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    sendAttempts = await AsyncQueryableExecuter.ToListAsync(
                        (await _webhookSendAttemptRepository.GetAllAsync())
                            .Where(attempt => attempt.WebhookEventId == webhookEventId)
                            .OrderByDescending(attempt => attempt.CreationTime)
                    );
                }

                await uow.CompleteAsync();
            }

            return sendAttempts;
        }

        public virtual List<WebhookSendAttempt> GetAllSendAttemptsByWebhookEventId(Guid? tenantId, Guid webhookEventId)
        {
            List<WebhookSendAttempt> sendAttempts;

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    sendAttempts = _webhookSendAttemptRepository.GetAll()
                        .Where(attempt => attempt.WebhookEventId == webhookEventId)
                        .OrderByDescending(attempt => attempt.CreationTime).ToList();
                }

                uow.Complete();
            }

            return sendAttempts;
        }
    }
}
