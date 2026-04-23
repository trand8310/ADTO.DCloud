using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public class NullDynamicPropertyValueStore : IDynamicPropertyValueStore
    {
        public static NullDynamicPropertyValueStore Instance = new NullDynamicPropertyValueStore();

        public DynamicPropertyValue Get(Guid id)
        {
            return default;
        }

        public Task<DynamicPropertyValue> GetAsync(Guid id)
        {
            return Task.FromResult<DynamicPropertyValue>(default);
        }

        public List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(Guid dynamicPropertyId)
        {
            return new List<DynamicPropertyValue>();
        }

        public Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(Guid dynamicPropertyId)
        {
            return Task.FromResult(new List<DynamicPropertyValue>());
        }

        public void Add(DynamicPropertyValue dynamicPropertyValue)
        {
        }

        public Task AddAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicPropertyValue dynamicPropertyValue)
        {
        }

        public Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            return Task.CompletedTask;
        }

        public void Delete(Guid id)
        {
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public void CleanValues(Guid dynamicPropertyId)
        {
        }

        public Task CleanValuesAsync(Guid dynamicPropertyId)
        {
            return Task.CompletedTask;
        }
    }
}