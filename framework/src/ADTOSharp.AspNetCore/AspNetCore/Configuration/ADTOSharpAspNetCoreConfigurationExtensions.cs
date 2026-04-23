using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.AspNetCore.Configuration;

/// <summary>
/// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ADTO ASP.NET Core module.
/// </summary>
public static class ADTOSharpAspNetCoreConfigurationExtensions
{
    /// <summary>
    /// Used to configure ADTO ASP.NET Core module.
    /// </summary>
    public static IADTOSharpAspNetCoreConfiguration ADTOSharpAspNetCore(this IModuleConfigurations configurations)
    {
        return configurations.ADTOSharpConfiguration.Get<IADTOSharpAspNetCoreConfiguration>();
    }
}