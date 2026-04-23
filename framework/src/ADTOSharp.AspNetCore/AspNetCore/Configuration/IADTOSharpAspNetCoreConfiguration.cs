using System;
using System.Collections.Generic;
using System.Reflection;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace ADTOSharp.AspNetCore.Configuration;

public interface IADTOSharpAspNetCoreConfiguration
{
    WrapResultAttribute DefaultWrapResultAttribute { get; }

    ResponseCacheAttribute DefaultResponseCacheAttributeForControllers { get; set; }

    ResponseCacheAttribute DefaultResponseCacheAttributeForAppServices { get; set; }

    UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

    List<Type> FormBodyBindingIgnoredTypes { get; }

    /// <summary>
    /// Default: true.
    /// </summary>
    bool IsValidationEnabledForControllers { get; set; }

    /// <summary>
    /// Used to enable/disable auditing for MVC controllers.
    /// Default: true.
    /// </summary>
    bool IsAuditingEnabled { get; set; }

    /// <summary>
    /// Default: true.
    /// </summary>
    bool SetNoCacheForAjaxResponses { get; set; }

    /// <summary>
    /// Default: false.
    /// </summary>
    bool UseMvcDateTimeFormatForAppServices { get; [Obsolete("Use InputDateTimeFormats and OutputDateTimeFormat instead.")] set; }

    /// <summary>
    /// Formats of input JSON date, Empty string means default format.
    /// </summary>
    List<string> InputDateTimeFormats { get; set; }

    /// <summary>
    /// Format of output json date, Null or empty string means default format.
    /// </summary>
    string OutputDateTimeFormat { get; set; }

    /// <summary>
    /// Used to add route config for modules.
    /// </summary>
    List<Action<IEndpointRouteBuilder>> EndpointConfiguration { get; }

    ADTOSharpControllerAssemblySettingBuilder CreateControllersForAppServices(
        Assembly assembly,
        string moduleName = ADTOSharpControllerAssemblySetting.DefaultServiceModuleName,
        bool useConventionalHttpVerbs = true
    );
}