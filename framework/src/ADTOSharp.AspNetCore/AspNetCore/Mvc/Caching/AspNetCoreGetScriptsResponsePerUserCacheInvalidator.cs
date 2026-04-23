using ADTOSharp.CachedUniqueKeys;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.Localization;

namespace ADTOSharp.AspNetCore.Mvc.Caching;

public class AspNetCoreGetScriptsResponsePerUserCacheInvalidator :
    IEventHandler<EntityChangedEventData<LanguageInfo>>,
    IEventHandler<EntityChangedEventData<SettingInfo>>,
    ITransientDependency
{
    private const string CacheName = "GetScriptsResponsePerUser";

    private readonly ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;

    public AspNetCoreGetScriptsResponsePerUserCacheInvalidator(ICachedUniqueKeyPerUser cachedUniqueKeyPerUser)
    {
        _cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
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