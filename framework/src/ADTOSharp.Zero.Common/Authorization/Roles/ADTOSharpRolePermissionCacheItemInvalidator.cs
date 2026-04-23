using ADTOSharp.Dependency;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Organizations;
using ADTOSharp.Runtime.Caching;
using System;

namespace ADTOSharp.Authorization.Roles
{
    public class ADTOSharpRolePermissionCacheItemInvalidator :
        IEventHandler<EntityChangedEventData<RolePermissionSetting>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        IEventHandler<EntityDeletedEventData<ADTOSharpRoleBase>>,
        ITransientDependency
    {
        private readonly ICacheManager _cacheManager;

        public ADTOSharpRolePermissionCacheItemInvalidator(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void HandleEvent(EntityChangedEventData<RolePermissionSetting> eventData)
        {
            var cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            var cacheKey = eventData.Entity.RoleId + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }

        public void HandleEvent(EntityDeletedEventData<ADTOSharpRoleBase> eventData)
        {
            var cacheKey = eventData.Entity.Id + "@" + (eventData.Entity.TenantId ?? Guid.Empty);
            _cacheManager.GetRolePermissionCache().Remove(cacheKey);
        }
    }
}