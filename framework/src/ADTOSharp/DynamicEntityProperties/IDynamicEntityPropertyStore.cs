using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyStore
    {
        DynamicEntityProperty Get(Guid id);

        Task<DynamicEntityProperty> GetAsync(Guid id);

        List<DynamicEntityProperty> GetAll();

        Task<List<DynamicEntityProperty>> GetAllAsync();

        List<DynamicEntityProperty> GetAll(string entityFullName);

        Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName);
        
        void Add(DynamicEntityProperty dynamicEntityProperty);

        Task AddAsync(DynamicEntityProperty dynamicEntityProperty);

        void Update(DynamicEntityProperty dynamicEntityProperty);

        Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty);

        void Delete(Guid id);

        Task DeleteAsync(Guid id);
    }
}
