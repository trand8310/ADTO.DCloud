using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ADTOSharp.Application.Editions;
using ADTOSharp.Application.Features;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Services;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Localization;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.UI;
using ADTOSharp.Zero;

namespace ADTOSharp.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// Implements domain logic for <see cref="ADTOSharpTenant{TUser}"/>.
    /// </summary>
    /// <typeparam name="TTenant">Type of the application Tenant</typeparam>
    /// <typeparam name="TUser">Type of the application User</typeparam>
    public class ADTOSharpTenantManager<TTenant, TUser> : IDomainService,
        IEventHandler<EntityChangedEventData<TTenant>>,
        IEventHandler<EntityDeletedEventData<Edition>>
        where TTenant : ADTOSharpTenant<TUser>
        where TUser : ADTOSharpUserBase
    {
        public ADTOSharpEditionManager EditionManager { get; set; }

        public ILocalizationManager LocalizationManager { get; set; }

        protected string LocalizationSourceName { get; set; }

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        protected IRepository<TTenant,Guid> TenantRepository { get; set; }

        protected IRepository<TenantFeatureSetting, Guid> TenantFeatureRepository { get; set; }

        private readonly IADTOSharpZeroFeatureValueStore _featureValueStore;

        public ADTOSharpTenantManager(
            IRepository<TTenant, Guid> tenantRepository, 
            IRepository<TenantFeatureSetting, Guid> tenantFeatureRepository,
            ADTOSharpEditionManager editionManager,
            IADTOSharpZeroFeatureValueStore featureValueStore)
        {
            _featureValueStore = featureValueStore;
            TenantRepository = tenantRepository;
            TenantFeatureRepository = tenantFeatureRepository;
            EditionManager = editionManager;
            LocalizationManager = NullLocalizationManager.Instance;
            LocalizationSourceName = ADTOSharpZeroConsts.LocalizationSourceName;
        }

        public virtual IQueryable<TTenant> Tenants { get { return TenantRepository.GetAll(); } }

        public virtual async Task CreateAsync(TTenant tenant)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await ValidateTenantAsync(tenant);

                if (await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenant.TenancyName) != null)
                {
                    throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
                }

                await TenantRepository.InsertAsync(tenant);
            });
        }

        public virtual void Create(TTenant tenant)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                ValidateTenant(tenant);

                if (TenantRepository.FirstOrDefault(t => t.TenancyName == tenant.TenancyName) != null)
                {
                    throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
                }

                TenantRepository.Insert(tenant);
            });
        }

        public virtual async Task UpdateAsync(TTenant tenant)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenant.TenancyName && t.Id != tenant.Id) != null)
                {
                    throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
                }

                await TenantRepository.UpdateAsync(tenant);
            });
        }

        public virtual void Update(TTenant tenant)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                if (TenantRepository.FirstOrDefault(t => t.TenancyName == tenant.TenancyName && t.Id != tenant.Id) != null)
                {
                    throw new UserFriendlyException(string.Format(L("TenancyNameIsAlreadyTaken"), tenant.TenancyName));
                }

                TenantRepository.Update(tenant);
            });
        }

        public virtual async Task<TTenant> FindByIdAsync(Guid id)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await TenantRepository.FirstOrDefaultAsync(id));
        }

        public virtual TTenant FindById(Guid id)
        {
            return UnitOfWorkManager.WithUnitOfWork(() => TenantRepository.FirstOrDefault(id));
        }

        public virtual async Task<TTenant> GetByIdAsync(Guid id)
        {
            var tenant = await FindByIdAsync(id);
            if (tenant == null)
            {
                throw new ADTOSharpException("There is no tenant with id: " + id);
            }

            return tenant;
        }

        public virtual TTenant GetById(Guid id)
        {
            var tenant = FindById(id);
            if (tenant == null)
            {
                throw new ADTOSharpException("There is no tenant with id: " + id);
            }

            return tenant;
        }

        public virtual async Task<TTenant> FindByTenancyNameAsync(string tenancyName)
        {
            return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
            });
        }

        public virtual TTenant FindByTenancyName(string tenancyName)
        {
            return UnitOfWorkManager.WithUnitOfWork(() =>
            {
                return TenantRepository.FirstOrDefault(t => t.TenancyName == tenancyName);
            });
        }

        public virtual async Task DeleteAsync(TTenant tenant)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await TenantRepository.DeleteAsync(tenant);
            });
        }

        public virtual void Delete(TTenant tenant)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                TenantRepository.Delete(tenant);
            });
        }

        public Task<string> GetFeatureValueOrNullAsync(Guid tenantId, string featureName)
        {
            return _featureValueStore.GetValueOrNullAsync(tenantId, featureName);
        }

        public string GetFeatureValueOrNull(Guid tenantId, string featureName)
        {
            return _featureValueStore.GetValueOrNull(tenantId, featureName);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(Guid tenantId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(tenantId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual IReadOnlyList<NameValue> GetFeatureValues(Guid tenantId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, GetFeatureValueOrNull(tenantId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(Guid tenantId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(tenantId, value.Name, value.Value);
            }
        }

        public virtual void SetFeatureValues(Guid tenantId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                SetFeatureValue(tenantId, value.Name, value.Value);
            }
        }

        public virtual async Task SetFeatureValueAsync(Guid tenantId, string featureName, string value)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await SetFeatureValueAsync(await GetByIdAsync(tenantId), featureName, value);
            });
        }

        public virtual void SetFeatureValue(Guid tenantId, string featureName, string value)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                SetFeatureValue(GetById(tenantId), featureName, value);
            });
        }

        public virtual async Task SetFeatureValueAsync(TTenant tenant, string featureName, string value)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                //No need to change if it's already equals to the current value
                if (await GetFeatureValueOrNullAsync(tenant.Id, featureName) == value)
                {
                    return;
                }

                //Get the current feature setting
                TenantFeatureSetting currentSetting;
                using (UnitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (UnitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    currentSetting = await TenantFeatureRepository.FirstOrDefaultAsync(f => f.Name == featureName);
                }

                //Get the feature
                var feature = FeatureManager.GetOrNull(featureName);
                if (feature == null)
                {
                    if (currentSetting != null)
                    {
                        await TenantFeatureRepository.DeleteAsync(currentSetting);
                    }
                    
                    return;
                }

                //Determine default value
                var defaultValue = tenant.EditionId.HasValue
                    ? (await EditionManager.GetFeatureValueOrNullAsync(tenant.EditionId.Value, featureName) ?? feature.DefaultValue)
                    : feature.DefaultValue;

                //No need to store value if it's default
                if (value == defaultValue)
                {
                    if (currentSetting != null)
                    {
                        await TenantFeatureRepository.DeleteAsync(currentSetting);
                    }
                    
                    return;
                }

                //Insert/update the feature value
                if (currentSetting == null)
                {
                    await TenantFeatureRepository.InsertAsync(new TenantFeatureSetting(tenant.Id, featureName, value));
                }
                else
                {
                    currentSetting.Value = value;
                }
            });
        }

        public virtual void SetFeatureValue(TTenant tenant, string featureName, string value)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                 //No need to change if it's already equals to the current value
                if (GetFeatureValueOrNull(tenant.Id, featureName) == value)
                {
                    return;
                }

                //Get the current feature setting
                TenantFeatureSetting currentSetting;
                using (UnitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (UnitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    currentSetting = TenantFeatureRepository.FirstOrDefault(f => f.Name == featureName);
                }

                //Get the feature
                var feature = FeatureManager.GetOrNull(featureName);
                if (feature == null)
                {
                    if (currentSetting != null)
                    {
                        TenantFeatureRepository.Delete(currentSetting);
                    }

                    return;
                }

                //Determine default value
                var defaultValue = tenant.EditionId.HasValue
                    ? (EditionManager.GetFeatureValueOrNull(tenant.EditionId.Value, featureName) ?? feature.DefaultValue)
                    : feature.DefaultValue;

                //No need to store value if it's default
                if (value == defaultValue)
                {
                    if (currentSetting != null)
                    {
                        TenantFeatureRepository.Delete(currentSetting);
                    }

                    return;
                }

                //Insert/update the feature value
                if (currentSetting == null)
                {
                    TenantFeatureRepository.Insert(new TenantFeatureSetting(tenant.Id, featureName, value));
                }
                else
                {
                    currentSetting.Value = value;
                }
            });
        }

        /// <summary>
        /// Resets all custom feature settings for a tenant.
        /// Tenant will have features according to it's edition.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        public virtual async Task ResetAllFeaturesAsync(Guid tenantId)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (UnitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await TenantFeatureRepository.DeleteAsync(f => f.TenantId == tenantId);
                }
            });
        }

        /// <summary>
        /// Resets all custom feature settings for a tenant.
        /// Tenant will have features according to it's edition.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        public virtual void ResetAllFeatures(Guid tenantId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (UnitOfWorkManager.Current.EnableFilter(ADTOSharpDataFilters.MayHaveTenant))
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    TenantFeatureRepository.Delete(f => f.TenantId == tenantId);
                }
            });
        }

        protected virtual async Task ValidateTenantAsync(TTenant tenant)
        {
            await ValidateTenancyNameAsync(tenant.TenancyName);
        }

        protected virtual void ValidateTenant(TTenant tenant)
        {
            ValidateTenancyName(tenant.TenancyName);
        }

        protected virtual Task ValidateTenancyNameAsync(string tenancyName)
        {
            if (!Regex.IsMatch(tenancyName, ADTOSharpTenant<TUser>.TenancyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidTenancyName"));
            }

            return Task.FromResult(0);
        }

        protected virtual void ValidateTenancyName(string tenancyName)
        {
            if (!Regex.IsMatch(tenancyName, ADTOSharpTenant<TUser>.TenancyNameRegex))
            {
                throw new UserFriendlyException(L("InvalidTenancyName"));
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

        public void HandleEvent(EntityChangedEventData<TTenant> eventData)
        {
            if (eventData.Entity.IsTransient())
            {
                return;
            }

            CacheManager.GetTenantFeatureCache().Remove(eventData.Entity.Id);
        }
        
        public virtual void HandleEvent(EntityDeletedEventData<Edition> eventData)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                var relatedTenants = TenantRepository.GetAllList(t => t.EditionId == eventData.Entity.Id);
                foreach (var relatedTenant in relatedTenants)
                {
                    relatedTenant.EditionId = null;
                }
            });
        }
    }
}
