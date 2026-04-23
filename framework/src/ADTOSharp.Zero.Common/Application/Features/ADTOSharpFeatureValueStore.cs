using System;
using System.Globalization;
using System.Threading.Tasks;
using ADTOSharp.Application.Editions;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using ADTOSharp.Zero;

namespace ADTOSharp.Application.Features
{
    /// <summary>
    /// Implements <see cref="IFeatureValueStore"/>.
    /// </summary>
    public class ADTOSharpFeatureValueStore<TTenant, TUser> :
        IADTOSharpZeroFeatureValueStore,
        ITransientDependency,
        IEventHandler<EntityChangingEventData<Edition>>,
        IEventHandler<EntityChangingEventData<EditionFeatureSetting>>,
        IEventHandler<EntityChangingEventData<TenantFeatureSetting>>

        where TTenant : ADTOSharpTenant<TUser>
        where TUser : ADTOSharpUserBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<TenantFeatureSetting, Guid> _tenantFeatureRepository;
        private readonly IRepository<TTenant,Guid> _tenantRepository;
        private readonly IRepository<EditionFeatureSetting, Guid> _editionFeatureRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ILocalizationManager LocalizationManager { get; set; }
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ADTOSharpFeatureValueStore{TTenant, TUser}"/> class.
        /// </summary>
        public ADTOSharpFeatureValueStore(
            ICacheManager cacheManager,
            IRepository<TenantFeatureSetting, Guid> tenantFeatureRepository,
            IRepository<TTenant, Guid> tenantRepository,
            IRepository<EditionFeatureSetting, Guid> editionFeatureRepository,
            IFeatureManager featureManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _cacheManager = cacheManager;
            _tenantFeatureRepository = tenantFeatureRepository;
            _tenantRepository = tenantRepository;
            _editionFeatureRepository = editionFeatureRepository;
            _featureManager = featureManager;
            _unitOfWorkManager = unitOfWorkManager;

            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = ADTOSharpZeroConsts.LocalizationSourceName;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetValueOrNullAsync(Guid tenantId, Feature feature)
        {
            return GetValueOrNullAsync(tenantId, feature.Name);
        }

        /// <inheritdoc/>
        public virtual string GetValueOrNull(Guid tenantId, Feature feature)
        {
            return GetValueOrNull(tenantId, feature.Name);
        }

        public virtual async Task<string> GetEditionValueOrNullAsync(Guid editionId, string featureName)
        {
            var cacheItem = await GetEditionFeatureCacheItemAsync(editionId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual string GetEditionValueOrNull(Guid editionId, string featureName)
        {
            var cacheItem = GetEditionFeatureCacheItem(editionId);
            return cacheItem.FeatureValues.GetOrDefault(featureName);
        }

        public virtual async Task<string> GetValueOrNullAsync(Guid tenantId, string featureName)
        {
            var cacheItem = await GetTenantFeatureCacheItemAsync(tenantId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            if (value != null)
            {
                return value;
            }

            if (cacheItem.EditionId.HasValue)
            {
                value = await GetEditionValueOrNullAsync(cacheItem.EditionId.Value, featureName);
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        public virtual string GetValueOrNull(Guid tenantId, string featureName)
        {
            var cacheItem = GetTenantFeatureCacheItem(tenantId);
            var value = cacheItem.FeatureValues.GetOrDefault(featureName);
            if (value != null)
            {
                return value;
            }

            if (cacheItem.EditionId.HasValue)
            {
                value = GetEditionValueOrNull(cacheItem.EditionId.Value, featureName);
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }
        
        public virtual async Task SetEditionFeatureValueAsync(Guid editionId, string featureName, string value)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    if (await GetEditionValueOrNullAsync(editionId, featureName) == value)
                    {
                        return;
                    }

                    var currentFeature = await _editionFeatureRepository.FirstOrDefaultAsync(f => f.EditionId == editionId && f.Name == featureName);

                    var feature = _featureManager.GetOrNull(featureName);
                    if (feature == null || feature.DefaultValue == value)
                    {
                        if (currentFeature != null)
                        {
                            await _editionFeatureRepository.DeleteAsync(currentFeature);
                        }

                        return;
                    }

                    if (!feature.InputType.Validator.IsValid(value))
                    {
                        throw new UserFriendlyException(string.Format(
                            L("InvalidFeatureValue"), feature.Name));
                    }

                    if (currentFeature == null)
                    {
                        await _editionFeatureRepository.InsertAsync(new EditionFeatureSetting(editionId, featureName, value));
                    }
                    else
                    {
                        currentFeature.Value = value;
                    }
                }
            });
        }
        
        public virtual void SetEditionFeatureValue(Guid editionId, string featureName, string value)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    if (GetEditionValueOrNull(editionId, featureName) == value)
                    {
                        return;
                    }

                    var currentFeature = _editionFeatureRepository.FirstOrDefault(f => f.EditionId == editionId && f.Name == featureName);

                    var feature = _featureManager.GetOrNull(featureName);
                    if (feature == null || feature.DefaultValue == value)
                    {
                        if (currentFeature != null)
                        {
                            _editionFeatureRepository.Delete(currentFeature);
                        }

                        return;
                    }

                    if (currentFeature == null)
                    {
                        _editionFeatureRepository.Insert(new EditionFeatureSetting(editionId, featureName, value));
                    }
                    else
                    {
                        currentFeature.Value = value;
                    }
                }
            });
        }

        protected virtual async Task<TenantFeatureCacheItem> GetTenantFeatureCacheItemAsync(Guid tenantId)
        {
            return await _cacheManager.GetTenantFeatureCache().GetAsync(tenantId, async () =>
            {
                TTenant tenant;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(null))
                    {
                        tenant = await _tenantRepository.GetAsync(tenantId);

                        await uow.CompleteAsync();
                    }
                }

                var newCacheItem = new TenantFeatureCacheItem { EditionId = tenant.EditionId };

                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var featureSettings = await _tenantFeatureRepository.GetAllListAsync();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        await uow.CompleteAsync();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual TenantFeatureCacheItem GetTenantFeatureCacheItem(Guid tenantId)
        {
            return _cacheManager.GetTenantFeatureCache().Get(tenantId, () =>
            {
                TTenant tenant;
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(null))
                    {
                        tenant = _tenantRepository.Get(tenantId);

                        uow.Complete();
                    }
                }

                var newCacheItem = new TenantFeatureCacheItem { EditionId = tenant.EditionId };

                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var featureSettings = _tenantFeatureRepository.GetAllList();
                        foreach (var featureSetting in featureSettings)
                        {
                            newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                        }

                        uow.Complete();
                    }
                }

                return newCacheItem;
            });
        }

        protected virtual async Task<EditionfeatureCacheItem> GetEditionFeatureCacheItemAsync(Guid editionId)
        {
            return await _cacheManager
                .GetEditionFeatureCache()
                .GetAsync(
                    editionId,
                    async () => await CreateEditionFeatureCacheItemAsync(editionId)
                );
        }

        protected virtual EditionfeatureCacheItem GetEditionFeatureCacheItem(Guid editionId)
        {
            return _cacheManager
                .GetEditionFeatureCache()
                .Get(
                    editionId,
                    () => CreateEditionFeatureCacheItem(editionId)
                );
        }

        protected virtual async Task<EditionfeatureCacheItem> CreateEditionFeatureCacheItemAsync(Guid editionId)
        {
            var newCacheItem = new EditionfeatureCacheItem();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var featureSettings = await _editionFeatureRepository.GetAllListAsync(f => f.EditionId == editionId);
                    foreach (var featureSetting in featureSettings)
                    {
                        newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                    }

                    await uow.CompleteAsync();
                }
            }

            return newCacheItem;
        }

        protected virtual EditionfeatureCacheItem CreateEditionFeatureCacheItem(Guid editionId)
        {
            var newCacheItem = new EditionfeatureCacheItem();

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var featureSettings = _editionFeatureRepository.GetAllList(f => f.EditionId == editionId);
                    foreach (var featureSetting in featureSettings)
                    {
                        newCacheItem.FeatureValues[featureSetting.Name] = featureSetting.Value;
                    }

                    uow.Complete();
                }
            }

            return newCacheItem;
        }

        public virtual void HandleEvent(EntityChangingEventData<EditionFeatureSetting> eventData)
        {
            _cacheManager.GetEditionFeatureCache().Remove(eventData.Entity.EditionId);
        }

        public virtual void HandleEvent(EntityChangingEventData<Edition> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            _cacheManager.GetEditionFeatureCache().Remove(eventData.Entity.Id);
        }

        public virtual void HandleEvent(EntityChangingEventData<TenantFeatureSetting> eventData)
        {
            if (eventData.Entity.TenantId.HasValue)
            {
                _cacheManager.GetTenantFeatureCache().Remove(eventData.Entity.TenantId.Value);
            }
        }

        protected virtual string L(string name)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name);
        }

        protected virtual string L(string name, CultureInfo cultureInfo)
        {
            return LocalizationManager.GetString(LocalizationSourceName, name, cultureInfo);
        }
    }
}
