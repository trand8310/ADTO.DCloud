using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public interface IDynamicPropertyValueManager
    {
        DynamicPropertyValue Get(Guid id);

        Task<DynamicPropertyValue> GetAsync(Guid id);

        List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(Guid dynamicPropertyId);

        Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(Guid dynamicPropertyId);

        void Add(DynamicPropertyValue dynamicPropertyValue);

        Task AddAsync(DynamicPropertyValue dynamicPropertyValue);

        void Update(DynamicPropertyValue dynamicPropertyValue);

        Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue);

        void Delete(Guid id);

        Task DeleteAsync(Guid id);

        void CleanValues(Guid dynamicPropertyId);

        Task CleanValuesAsync(Guid dynamicPropertyId);
    }
}
