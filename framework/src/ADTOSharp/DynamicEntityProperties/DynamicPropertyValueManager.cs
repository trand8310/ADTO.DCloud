using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Dependency;

namespace ADTOSharp.DynamicEntityProperties
{
    public class DynamicPropertyValueManager : IDynamicPropertyValueManager, ITransientDependency
    {
        private readonly IDynamicPropertyPermissionChecker _dynamicPropertyPermissionChecker;

        public IDynamicPropertyValueStore DynamicPropertyValueStore { get; set; }

        public DynamicPropertyValueManager(IDynamicPropertyPermissionChecker dynamicPropertyPermissionChecker)
        {
            _dynamicPropertyPermissionChecker = dynamicPropertyPermissionChecker;
            DynamicPropertyValueStore = NullDynamicPropertyValueStore.Instance;
        }

        public virtual DynamicPropertyValue Get(Guid id)
        {
            var val = DynamicPropertyValueStore.Get(id);
            _dynamicPropertyPermissionChecker.CheckPermission(val.DynamicPropertyId);
            return val;
        }

        public virtual async Task<DynamicPropertyValue> GetAsync(Guid id)
        {
            var val = await DynamicPropertyValueStore.GetAsync(id);
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(val.DynamicPropertyId);
            return val;
        }

        public virtual List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(Guid dynamicPropertyId)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyId);
            return DynamicPropertyValueStore.GetAllValuesOfDynamicProperty(dynamicPropertyId);
        }

        public virtual async Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(Guid dynamicPropertyId)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyId);
            return await DynamicPropertyValueStore.GetAllValuesOfDynamicPropertyAsync(dynamicPropertyId);
        }

        public virtual void Add(DynamicPropertyValue dynamicPropertyValue)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyValue.DynamicPropertyId);
            DynamicPropertyValueStore.Add(dynamicPropertyValue);
        }

        public virtual async Task AddAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyValue.DynamicPropertyId);
            await DynamicPropertyValueStore.AddAsync(dynamicPropertyValue);
        }

        public virtual void Update(DynamicPropertyValue dynamicPropertyValue)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyValue.DynamicPropertyId);
            DynamicPropertyValueStore.Update(dynamicPropertyValue);
        }

        public virtual async Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyValue.DynamicPropertyId);
            await DynamicPropertyValueStore.UpdateAsync(dynamicPropertyValue);
        }

        public virtual void Delete(Guid id)
        {
            var val = Get(id);
            if (val != null)//Get checks permission, no need to check it again  
            {
                DynamicPropertyValueStore.Delete(id);
            }
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var val = await GetAsync(id);
            if (val != null)//Get checks permission, no need to check it again
            {
                await DynamicPropertyValueStore.DeleteAsync(id);
            }
        }

        public virtual void CleanValues(Guid dynamicPropertyId)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyId);
            DynamicPropertyValueStore.CleanValues(dynamicPropertyId);
        }

        public virtual async Task CleanValuesAsync(Guid dynamicPropertyId)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyId);
            await DynamicPropertyValueStore.CleanValuesAsync(dynamicPropertyId);
        }
    }
}
