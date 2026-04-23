using System;

namespace ADTOSharp.AspNetCore.Mvc.Caching;

public interface IGetScriptsResponsePerUserConfiguration
{
    bool IsEnabled { get; set; }

    TimeSpan MaxAge { get; set; }
}