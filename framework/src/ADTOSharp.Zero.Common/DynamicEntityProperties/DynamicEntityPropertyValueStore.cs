using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq;

namespace ADTOSharp.DynamicEntityProperties
{
    public class DynamicEntityPropertyValueStore : IDynamicEntityPropertyValueStore, ITransientDependency
    {
        private readonly IRepository<DynamicEntityPropertyValue, Guid> _dynamicEntityPropertyValueRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public DynamicEntityPropertyValueStore(
            IRepository<DynamicEntityPropertyValue, Guid> dynamicEntityPropertyValueRepository,
            IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _dynamicEntityPropertyValueRepository = dynamicEntityPropertyValueRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public virtual DynamicEntityPropertyValue Get(Guid id)
        {
            return _dynamicEntityPropertyValueRepository.Get(id);
        }

        public virtual Task<DynamicEntityPropertyValue> GetAsync(Guid id)
        {
            return _dynamicEntityPropertyValueRepository.GetAsync(id);
        }

        public virtual void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
            _dynamicEntityPropertyValueRepository.Insert(dynamicEntityPropertyValue);
        }

        public virtual Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
            return _dynamicEntityPropertyValueRepository.InsertAsync(dynamicEntityPropertyValue);
        }

        public virtual void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
            _dynamicEntityPropertyValueRepository.Update(dynamicEntityPropertyValue);
        }

        public virtual Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
            return _dynamicEntityPropertyValueRepository.UpdateAsync(dynamicEntityPropertyValue);
        }

        public virtual void Delete(Guid id)
        {
            _dynamicEntityPropertyValueRepository.Delete(id);
        }

        public virtual Task DeleteAsync(Guid id)
        {
            return _dynamicEntityPropertyValueRepository.DeleteAsync(id);
        }

        public virtual List<DynamicEntityPropertyValue> GetValues(Guid dynamicEntityPropertyId, string entityId)
        {
            return _dynamicEntityPropertyValueRepository.GetAll().Where(val =>
                val.EntityId == entityId && val.DynamicEntityPropertyId == dynamicEntityPropertyId).ToList();
        }

        public virtual async Task<List<DynamicEntityPropertyValue>> GetValuesAsync(Guid dynamicEntityPropertyId,
            string entityId)
        {
            return await _asyncQueryableExecuter.ToListAsync(
                (await _dynamicEntityPropertyValueRepository.GetAllAsync())
                    .Where(val => val.EntityId == entityId && val.DynamicEntityPropertyId == dynamicEntityPropertyId)
            );
        }

        public List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId)
        {
            return _dynamicEntityPropertyValueRepository.GetAll()
                .Where(val => val.EntityId == entityId && val.DynamicEntityProperty.EntityFullName == entityFullName)
                .ToList();
        }

        public virtual async Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId)
        {
            return await _asyncQueryableExecuter.ToListAsync(
                (await _dynamicEntityPropertyValueRepository.GetAllAsync())
                    .Where(val =>
                        val.EntityId == entityId && val.DynamicEntityProperty.EntityFullName == entityFullName));
        }

        public List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, Guid dynamicPropertyId)
        {
            return _dynamicEntityPropertyValueRepository.GetAll()
                .Where(val =>
                    val.EntityId == entityId &&
                    val.DynamicEntityProperty.EntityFullName == entityFullName &&
                    val.DynamicEntityProperty.DynamicPropertyId == dynamicPropertyId
                )
                .ToList();
        }

        public virtual async Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId,
            Guid dynamicPropertyId)
        {
            return await _asyncQueryableExecuter.ToListAsync(
                (await _dynamicEntityPropertyValueRepository.GetAllAsync())
                    .Where(val =>
                        val.EntityId == entityId &&
                        val.DynamicEntityProperty.EntityFullName == entityFullName &&
                        val.DynamicEntityProperty.DynamicPropertyId == dynamicPropertyId
                    ));
        }

        public virtual void CleanValues(Guid dynamicEntityPropertyId, string entityId)
        {
            var list = _dynamicEntityPropertyValueRepository.GetAll().Where(val =>
                val.EntityId == entityId && val.DynamicEntityPropertyId == dynamicEntityPropertyId).ToList();

            foreach (var dynamicEntityPropertyValue in list)
            {
                _dynamicEntityPropertyValueRepository.Delete(dynamicEntityPropertyValue);
            }
        }

        public virtual async Task CleanValuesAsync(Guid dynamicEntityPropertyId, string entityId)
        {
            var list = await _asyncQueryableExecuter.ToListAsync(
                (await _dynamicEntityPropertyValueRepository.GetAllAsync())
                    .Where( val =>
                        val.EntityId == entityId && val.DynamicEntityPropertyId == dynamicEntityPropertyId));

            foreach (var dynamicEntityPropertyValue in list)
            {
                await _dynamicEntityPropertyValueRepository.DeleteAsync(dynamicEntityPropertyValue);
            }
        }
    }
}
