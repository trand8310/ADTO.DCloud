using ADTOSharp.Application.Features;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Services;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTOSharp.Application.Editions
{
    public class ADTOSharpEditionManager : IDomainService
    {
        private readonly IADTOSharpZeroFeatureValueStore _featureValueStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        
        public IQueryable<Edition> Editions => EditionRepository.GetAll();

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        protected IRepository<Edition,Guid> EditionRepository { get; set; }

        public ADTOSharpEditionManager(
            IRepository<Edition, Guid> editionRepository,
            IADTOSharpZeroFeatureValueStore featureValueStore, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _featureValueStore = featureValueStore;
            _unitOfWorkManager = unitOfWorkManager;
            EditionRepository = editionRepository;
        }

        public Task<IQueryable<Edition>> GetEditionsAsync 
            => EditionRepository.GetAllAsync();

        public virtual Task<string> GetFeatureValueOrNullAsync(Guid editionId, string featureName)
        {
            return _featureValueStore.GetEditionValueOrNullAsync(editionId, featureName);
        }

        public virtual string GetFeatureValueOrNull(Guid editionId, string featureName)
        {
            return _featureValueStore.GetEditionValueOrNull(editionId, featureName);
        }

        public virtual Task SetFeatureValueAsync(Guid editionId, string featureName, string value)
        {
            return _featureValueStore.SetEditionFeatureValueAsync(editionId, featureName, value);
        }

        public virtual void SetFeatureValue(Guid editionId, string featureName, string value)
        {
            _featureValueStore.SetEditionFeatureValue(editionId, featureName, value);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(Guid editionId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(editionId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual IReadOnlyList<NameValue> GetFeatureValues(Guid editionId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, GetFeatureValueOrNull(editionId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(Guid editionId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(editionId, value.Name, value.Value);
            }
        }

        public virtual void SetFeatureValues(Guid editionId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                SetFeatureValue(editionId, value.Name, value.Value);
            }
        }

        public virtual async Task CreateAsync(Edition edition)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await EditionRepository.InsertAsync(edition)
            );
        }

        public virtual void Create(Edition edition)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                EditionRepository.Insert(edition);
            });
        }

        public virtual async Task<Edition> FindByNameAsync(string name)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await EditionRepository.FirstOrDefaultAsync(edition => edition.Name == name);
            });
        }

        public virtual Edition FindByName(string name)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return EditionRepository.FirstOrDefault(edition => edition.Name == name);
            });
        }

        public virtual async Task<Edition> FindByIdAsync(Guid id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await EditionRepository.FirstOrDefaultAsync(id)
            );
        }

        public virtual Edition FindById(Guid id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
                EditionRepository.FirstOrDefault(id)
            );
        }

        public virtual async Task<Edition> GetByIdAsync(Guid id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await EditionRepository.GetAsync(id)
            );
        }

        public virtual Edition GetById(Guid id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() => EditionRepository.Get(id));
        }

        public virtual async Task DeleteAsync(Edition edition)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await EditionRepository.DeleteAsync(edition));
        }

        public virtual void Delete(Edition edition)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                EditionRepository.Delete(edition);
            });
        }
    }
}
