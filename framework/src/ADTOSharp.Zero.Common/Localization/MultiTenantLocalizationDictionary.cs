using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Extensions;
using ADTOSharp.Localization.Dictionaries;
using ADTOSharp.Runtime.Caching;
using ADTOSharp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace ADTOSharp.Localization
{
    /// <summary>
    /// Implements <see cref="IMultiTenantLocalizationDictionary"/>.
    /// </summary>
    public class MultiTenantLocalizationDictionary :
        IMultiTenantLocalizationDictionary
    {
        private readonly string _sourceName;
        private readonly ILocalizationDictionary _internalDictionary;
        private readonly IRepository<ApplicationLanguageText, Guid> _customLocalizationRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IADTOSharpSession _session;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionary"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionary(
            string sourceName,
            ILocalizationDictionary internalDictionary,
            IRepository<ApplicationLanguageText, Guid> customLocalizationRepository,
            ICacheManager cacheManager,
            IADTOSharpSession session,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _sourceName = sourceName;
            _internalDictionary = internalDictionary;
            _customLocalizationRepository = customLocalizationRepository;
            _cacheManager = cacheManager;
            _session = session;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public CultureInfo CultureInfo
        {
            get { return _internalDictionary.CultureInfo; }
        }

        public string this[string name]
        {
            get { return _internalDictionary[name]; }
            set { _internalDictionary[name] = value; }
        }

        public string TryGetKey(Guid? tenantId, string value)
        {
            //Get cache
            var cache = _cacheManager.GetMultiTenantLocalizationDictionaryCache();

            //Get for current tenant
            var dictionary = cache.Get(CalculateCacheKey(tenantId), () => GetAllValuesFromDatabase(tenantId));
            var foundValue = dictionary.Values.FirstOrDefault(x => x == value);
            if (foundValue != null)
            {
                return foundValue;
            }

            //Fall back to host
            if (tenantId != null)
            {
                dictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                foundValue = dictionary.Values.FirstOrDefault(x => x == value);
                if (foundValue != null)
                {
                    return foundValue;
                }
            }

            //Not found in database, fall back to internal dictionary
            var internalLocalizedString = _internalDictionary.TryGetKey(value);
            if (internalLocalizedString != null)
            {
                return internalLocalizedString;
            }

            //Not found at all
            return null;
        }

        public string TryGetKey(string value)
        {
            return TryGetKey(_session.TenantId, value);
        }

        public LocalizedString GetOrNull(string name)
        {
            return GetOrNull(_session.TenantId, name);
        }

        public IReadOnlyList<LocalizedString> GetStringsOrNull(List<string> names)
        {
            return GetStringsOrNull(_session.TenantId, names);
        }

        public LocalizedString GetOrNull(Guid? tenantId, string name)
        {
            //Get cache
            var cache = _cacheManager.GetMultiTenantLocalizationDictionaryCache();

            //Get for current tenant
            var dictionary = cache.Get(CalculateCacheKey(tenantId), () => GetAllValuesFromDatabase(tenantId));
            var value = dictionary.GetOrDefault(name);
            if (value != null)
            {
                return new LocalizedString(name, value, CultureInfo);
            }

            //Fall back to host
            if (tenantId != null)
            {
                dictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                value = dictionary.GetOrDefault(name);
                if (value != null)
                {
                    return new LocalizedString(name, value, CultureInfo);
                }
            }

            //Not found in database, fall back to internal dictionary
            var internalLocalizedString = _internalDictionary.GetOrNull(name);
            if (internalLocalizedString != null)
            {
                return internalLocalizedString;
            }

            //Not found at all
            return null;
        }

        public IReadOnlyList<LocalizedString> GetStringsOrNull(Guid? tenantId, List<string> names)
        {
            //Get cache
            var cache = _cacheManager.GetMultiTenantLocalizationDictionaryCache();

            //Create a temp dictionary to build (by underlying dictionary)
            var dictionary = new OrderedDictionary<string, LocalizedString>();

            foreach (var localizedString in _internalDictionary.GetStringsOrNull(names))
            {
                dictionary[localizedString.Name] = localizedString;
            }

            //Override by host
            if (tenantId != null)
            {
                var defaultDictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                foreach (var keyValue in defaultDictionary.Where(x => names.Contains(x.Key)))
                {
                    dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
                }
            }

            //Override by tenant
            var tenantDictionary = cache.Get(CalculateCacheKey(tenantId), () => GetAllValuesFromDatabase(tenantId));
            foreach (var keyValue in tenantDictionary.Where(x => names.Contains(x.Key)))
            {
                dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
            }

            return dictionary.Values.ToImmutableList();
        }


        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(_session.TenantId);
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(Guid? tenantId)
        {
            //Get cache
            var cache = _cacheManager.GetMultiTenantLocalizationDictionaryCache();

            //Create a temp dictionary to build (by underlying dictionary)
            var dictionary = new OrderedDictionary<string, LocalizedString>();

            foreach (var localizedString in _internalDictionary.GetAllStrings())
            {
                dictionary[localizedString.Name] = localizedString;
            }

            //Override by host
            if (tenantId != null)
            {
                var defaultDictionary = cache.Get(CalculateCacheKey(null), () => GetAllValuesFromDatabase(null));
                foreach (var keyValue in defaultDictionary)
                {
                    dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
                }
            }

            //Override by tenant
            var tenantDictionary = cache.Get(CalculateCacheKey(tenantId), () => GetAllValuesFromDatabase(tenantId));
            foreach (var keyValue in tenantDictionary)
            {
                dictionary[keyValue.Key] = new LocalizedString(keyValue.Key, keyValue.Value, CultureInfo);
            }

            return dictionary.Values.ToImmutableList();
        }

        private string CalculateCacheKey(Guid? tenantId)
        {
            return MultiTenantLocalizationDictionaryCacheHelper.CalculateCacheKey(
                tenantId,
                _sourceName,
                CultureInfo.Name
            );
        }

        /// <summary>
        /// 重数据库加载所有的资源KEY,VALUE值,按ID的序顺加入,可以让新增加的值永远在最后面
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        protected virtual OrderedDictionary<string, string> GetAllValuesFromDatabase(Guid? tenantId)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var list = _customLocalizationRepository
                     .GetAll()
                     .Where(l => l.Source == _sourceName &&
                                 l.LanguageName == CultureInfo.Name)

                     //.OrderBy(l => l.Id)

                     .Select(l => new { l.Key, l.Value })
                     .ToList();

                    return list.ToOrderedDictionary(l => l.Key, l => l.Value);
                }
            });


 
        }
    }
}
