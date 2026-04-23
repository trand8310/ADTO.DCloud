using System;
using System.Collections.Generic;
using System.Reflection;
using ADTOSharp.Dependency;

namespace ADTOSharp.HtmlSanitizer.Configuration;

public interface IHtmlSanitizerConfiguration
{
    bool IsEnabledForGetRequests { get; set; }

    List<Func<MethodInfo, bool>> Selectors { get; set; }

    bool KeepChildNodes { get; set; }
}