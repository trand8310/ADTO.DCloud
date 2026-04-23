using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public class NullDynamicEntityPropertyValueStore : IDynamicEntityPropertyValueStore
    {
        public static NullDynamicEntityPropertyValueStore Instance = new NullDynamicEntityPropertyValueStore();

        public DynamicEntityPropertyValue Get(Guid id)
        {
            return default;
        }

        public Task<DynamicEntityPropertyValue> GetAsync(Guid id)
        {
            return Task.FromResult<DynamicEntityPropertyValue>(default);
        }

        public void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
        }

        public Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
        }

        public Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue)
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

        public List<DynamicEntityPropertyValue> GetValues(Guid dynamicEntityPropertyId, string entityId)
        {
            return new List<DynamicEntityPropertyValue>();
        }

        public Task<List<DynamicEntityPropertyValue>> GetValuesAsync(Guid dynamicEntityPropertyId, string entityId)
        {
            return Task.FromResult(new List<DynamicEntityPropertyValue>());
        }

        public List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId)
        {
            return new List<DynamicEntityPropertyValue>();
        }

        public Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId)
        {
            return Task.FromResult(new List<DynamicEntityPropertyValue>());
        }

        public List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, Guid dynamicPropertyId)
        {
            return new List<DynamicEntityPropertyValue>();
        }

        public Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId, Guid dynamicPropertyId)
        {
            return Task.FromResult(new List<DynamicEntityPropertyValue>());
        }

        public void CleanValues(Guid dynamicEntityPropertyId, string entityId)
        {
        }

        public Task CleanValuesAsync(Guid dynamicEntityPropertyId, string entityId)
        {
            return Task.CompletedTask;
        }
    }
}
