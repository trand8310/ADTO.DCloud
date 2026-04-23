using System;
using System.Threading.Tasks;

namespace ADTOSharp.DynamicEntityProperties
{
    public interface IDynamicPropertyManager
    {
        DynamicProperty Get(Guid id);

        Task<DynamicProperty> GetAsync(Guid id);

        DynamicProperty Get(string propertyName);

        Task<DynamicProperty> GetAsync(string propertyName);

        DynamicProperty Add(DynamicProperty dynamicProperty);

        Task<DynamicProperty> AddAsync(DynamicProperty dynamicProperty);

        DynamicProperty Update(DynamicProperty dynamicProperty);

        Task<DynamicProperty> UpdateAsync(DynamicProperty dynamicProperty);

        void Delete(Guid id);

        Task DeleteAsync(Guid id);
    }
}
