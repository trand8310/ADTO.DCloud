using System;
using System.Threading.Tasks;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Domain.Entities.Caching
{
    public abstract class MultiTenancyEntityCache<TEntity, TCacheItem, TPrimaryKey> :
        EntityCacheBase<TEntity, TCacheItem, TPrimaryKey>,
        IEventHandler<EntityChangedEventData<TEntity>>,
        IMultiTenancyEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ITypedCache<string, TCacheItem> InternalCache => CacheManager.GetCache<string, TCacheItem>(CacheName);

        public IADTOSharpSession ADTOSharpSession { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MultiTenancyEntityCache(
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TEntity, TPrimaryKey> repository,
            string cacheName = null)
            : base(
                cacheManager,
                repository,
                unitOfWorkManager,
                cacheName)
        {
            _unitOfWorkManager = unitOfWorkManager;

            ADTOSharpSession = NullADTOSharpSession.Instance;
        }

        public override TCacheItem Get(TPrimaryKey id)
        {
            return InternalCache.Get(GetCacheKey(id), () => GetCacheItemFromDataSource(id));
        }

        public override Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            return InternalCache.GetAsync(GetCacheKey(id), async () => await GetCacheItemFromDataSourceAsync(id));
        }

        public virtual void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            InternalCache.Remove(GetCacheKey(eventData.Entity));
        }

        protected virtual Guid? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return ADTOSharpSession.TenantId;
        }

        public virtual string GetCacheKey(TPrimaryKey id)
        {
            return GetCacheKey(id, GetCurrentTenantId());
        }

        public virtual string GetCacheKey(TPrimaryKey id, Guid? tenantId)
        {
            return id + "@" + (tenantId ?? Guid.Empty);
        }

        protected abstract string GetCacheKey(TEntity entity);

        public override string ToString()
        {
            return $"MultiTenancyEntityCache {CacheName}";
        }
    }
}
