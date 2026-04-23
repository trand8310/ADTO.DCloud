using System;
using System.Threading.Tasks;
using ADTOSharp.Runtime.Caching;

namespace ADTO.DCloud.Storage;
/// <summary>
/// 临时缓存文件管理
/// </summary>
public class TempFileCacheManager : ITempFileCacheManager
{
    private const string TempFileCacheName = "TempFileCacheName";

    private readonly ITypedCache<string, TempFileInfo> _cache;

    public TempFileCacheManager(ICacheManager cacheManager)
    {
        _cache = cacheManager.GetCache<string, TempFileInfo>(TempFileCacheName);
    }

    public void SetFile(string token, byte[] content)
    {
        _cache.Set(token, new TempFileInfo(content), TimeSpan.FromMinutes(30)); //默认30分钟有效
    }

    public Task SetFileAsync(string token, byte[] content)
    {
        return _cache.SetAsync(token, new TempFileInfo(content), TimeSpan.FromMinutes(30));
    }

    public Task SetFileAsync(string token, TempFileInfo info)
    {
        return _cache.SetAsync(token, info, TimeSpan.FromMinutes(30));
    }


    public byte[] GetFile(string token)
    {
        var cache = _cache.GetOrDefault(token);
        return cache?.File;
    }

    public void SetFile(string token, TempFileInfo info)
    {
        _cache.Set(token, info, TimeSpan.FromMinutes(30)); // 默认认30分钟有效
    }

    public TempFileInfo GetFileInfo(string token)
    {
        return _cache.GetOrDefault(token);
    }
}