using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.AutoMapper;

/// <summary>
/// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ADTOSharp.AutoMapper module.
/// </summary>
public static class ADTOSharpAutoMapperConfigurationExtensions
{
    /// <summary>
    /// Used to configure ADTOSharp.AutoMapper module.
    /// </summary>
    public static IADTOSharpAutoMapperConfiguration ADTOSharpAutoMapper(this IModuleConfigurations configurations)
    {
        return configurations.ADTOSharpConfiguration.Get<IADTOSharpAutoMapperConfiguration>();
    }
}