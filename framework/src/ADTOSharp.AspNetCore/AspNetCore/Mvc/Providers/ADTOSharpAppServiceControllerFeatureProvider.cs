using System.Linq;
using System.Reflection;
using ADTOSharp.Application.Services;
using ADTOSharp.AspNetCore.Configuration;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Dependency;
using ADTOSharp.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ADTOSharp.AspNetCore.Mvc.Providers;

/// <summary>
/// Used to add application services as controller.
/// </summary>
public class ADTOSharpAppServiceControllerFeatureProvider : ControllerFeatureProvider
{
    private readonly IIocResolver _iocResolver;

    public ADTOSharpAppServiceControllerFeatureProvider(IIocResolver iocResolver)
    {
        _iocResolver = iocResolver;
    }

    protected override bool IsController(TypeInfo typeInfo)
    {
        var type = typeInfo.AsType();

        if (!typeof(IApplicationService).IsAssignableFrom(type) ||
            !typeInfo.IsPublic || typeInfo.IsAbstract || typeInfo.IsGenericType)
        {
            return false;
        }

        var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(typeInfo);

        if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type))
        {
            return false;
        }

        var settings = _iocResolver.Resolve<ADTOSharpAspNetCoreConfiguration>().ControllerAssemblySettings.GetSettings(type);
        return settings.Any(setting => setting.TypePredicate(type));
    }
}