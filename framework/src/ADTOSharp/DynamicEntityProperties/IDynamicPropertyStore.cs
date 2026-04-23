using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public interface IDynamicPropertyStore
    {
        DynamicProperty Get(Guid id);

        Task<DynamicProperty> GetAsync(Guid id);

        DynamicProperty Get(string propertyName);

        Task<DynamicProperty> GetAsync(string propertyName);

        List<DynamicProperty> GetAll();

        Task<List<DynamicProperty>> GetAllAsync();

        void Add(DynamicProperty dynamicProperty);

        Task AddAsync(DynamicProperty dynamicProperty);

        void Update(DynamicProperty dynamicProperty);

        Task UpdateAsync(DynamicProperty dynamicProperty);

        void Delete(Guid id);

        Task DeleteAsync(Guid id);
    }
}
