using System;

namespace ADTOSharp.Runtime.Caching.Redis.RealTime;

public static class OnlineClientInstanceIdProvider
{
    private static readonly Lazy<string> InstanceIdLazy = new(CreateInstanceId);

    public static string GetInstanceId()
    {
        return InstanceIdLazy.Value;
    }

    private static string CreateInstanceId()
    {
        var preferred = Environment.GetEnvironmentVariable("POD_NAME")
                        ?? Environment.GetEnvironmentVariable("HOSTNAME")
                        ?? Environment.MachineName;

        return preferred?.Trim().ToLowerInvariant() ?? Guid.NewGuid().ToString("N");
    }
}
