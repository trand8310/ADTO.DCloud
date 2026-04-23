using ADTOSharp.Authorization.Users;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Localization;
using ADTOSharp.Organizations;
using ADTOSharp.CachedUniqueKeys;

namespace ADTOSharp.Caching
{
    public class GetScriptsResponsePerUserCacheInvalidator :
        IEventHandler<EntityChangedEventData<UserPermissionSetting>>,
        IEventHandler<EntityChangedEventData<UserRole>>,
        IEventHandler<EntityChangedEventData<UserOrganizationUnit>>,
        IEventHandler<EntityDeletedEventData<ADTOSharpUserBase>>,
        IEventHandler<EntityChangedEventData<OrganizationUnitRole>>,
        IEventHandler<EntityChangedEventData<LanguageInfo>>,
        IEventHandler<EntityChangedEventData<SettingInfo>>,
        ITransientDependency
    {
        private const string CacheName = "GetScriptsResponsePerUser";

        private readonly ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;

        public GetScriptsResponsePerUserCacheInvalidator(ICachedUniqueKeyPerUser cachedUniqueKeyPerUser)
        {
            _cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
        }

        public void HandleEvent(EntityChangedEventData<UserPermissionSetting> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.TenantId, eventData.Entity.UserId);
        }

        public void HandleEvent(EntityChangedEventData<UserRole> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.TenantId, eventData.Entity.UserId);
        }

        public void HandleEvent(EntityChangedEventData<UserOrganizationUnit> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.TenantId, eventData.Entity.UserId);
        }

        public void HandleEvent(EntityDeletedEventData<ADTOSharpUserBase> eventData)
        {
            _cachedUniqueKeyPerUser.RemoveKey(CacheName, eventData.Entity.TenantId, eventData.Entity.Id);
        }

        public void HandleEvent(EntityChangedEventData<OrganizationUnitRole> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }

        public void HandleEvent(EntityChangedEventData<LanguageInfo> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }

        public void HandleEvent(EntityChangedEventData<SettingInfo> eventData)
        {
            _cachedUniqueKeyPerUser.ClearCache(CacheName);
        }
    }
}