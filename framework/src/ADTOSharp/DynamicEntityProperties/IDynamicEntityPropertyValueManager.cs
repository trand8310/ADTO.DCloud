using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyValueManager
    {
        DynamicEntityPropertyValue Get(Guid id);

        Task<DynamicEntityPropertyValue> GetAsync(Guid id);

        void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        void Delete(Guid id);

        Task DeleteAsync(Guid id);

        List<DynamicEntityPropertyValue> GetValues(Guid dynamicEntityPropertyId, string entityId);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(Guid dynamicEntityPropertyId, string entityId);

        List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId);

        List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, Guid dynamicPropertyId);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId, Guid dynamicPropertyId);

        List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, string propertyName);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId, string propertyName);

        void CleanValues(Guid dynamicEntityPropertyId, string entityId);

        Task CleanValuesAsync(Guid dynamicEntityPropertyId, string entityId);
    }
}
