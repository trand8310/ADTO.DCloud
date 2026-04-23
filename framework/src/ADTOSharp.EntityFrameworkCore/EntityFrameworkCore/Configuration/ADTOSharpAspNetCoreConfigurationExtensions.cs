using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.EntityFrameworkCore.Configuration;

/// <summary>
/// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ADTO EntityFramework Core module.
/// </summary>
public static class ADTOSharpEfCoreConfigurationExtensions
{
    /// <summary>
    /// Used to configure ADTO EntityFramework Core module.
    /// </summary>
    public static IADTOSharpEfCoreConfiguration ADTOSharpEfCore(this IModuleConfigurations configurations)
    {
        return configurations.ADTOSharpConfiguration.Get<IADTOSharpEfCoreConfiguration>();
    }
}