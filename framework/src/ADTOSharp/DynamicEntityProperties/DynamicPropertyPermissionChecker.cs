using System;
using System.Threading.Tasks;
using ADTOSharp.Authorization;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Extensions;

namespace ADTOSharp.DynamicEntityProperties
{
    public class DynamicPropertyPermissionChecker : IDynamicPropertyPermissionChecker, ITransientDependency
    {
        private readonly IPermissionChecker _permissionChecker;
        private readonly IDynamicPropertyManager _dynamicPropertyManager;

        public DynamicPropertyPermissionChecker(
            IPermissionChecker permissionChecker,
            IDynamicPropertyManager dynamicPropertyManager
            )
        {
            _permissionChecker = permissionChecker;
            _dynamicPropertyManager = dynamicPropertyManager;
        }

        public void CheckPermission(Guid dynamicPropertyId)
        {
            var dynamicProperty = _dynamicPropertyManager.Get(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!_permissionChecker.IsGranted(dynamicProperty.Permission))
            {
                throw new Exception($"Permission \"{dynamicProperty.Permission}\" is not granted");
            }
        }

        public async Task CheckPermissionAsync(Guid dynamicPropertyId)
        {
            var dynamicProperty = await _dynamicPropertyManager.GetAsync(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!await _permissionChecker.IsGrantedAsync(dynamicProperty.Permission))
            {
                throw new Exception($"Permission \"{dynamicProperty.Permission}\" is not granted");
            }
        }

        public bool IsGranted(Guid dynamicPropertyId)
        {
            var dynamicProperty = _dynamicPropertyManager.Get(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return true;
            }

            return _permissionChecker.IsGranted(dynamicProperty.Permission);
        }

        public async Task<bool> IsGrantedAsync(Guid dynamicPropertyId)
        {
            var dynamicProperty = await _dynamicPropertyManager.GetAsync(dynamicPropertyId);
            if (dynamicProperty == null)
            {
                throw new EntityNotFoundException(typeof(DynamicProperty), dynamicPropertyId);
            }

            if (dynamicProperty.Permission.IsNullOrWhiteSpace())
            {
                return true;
            }

            return await _permissionChecker.IsGrantedAsync(dynamicProperty.Permission);
        }
    }
}
