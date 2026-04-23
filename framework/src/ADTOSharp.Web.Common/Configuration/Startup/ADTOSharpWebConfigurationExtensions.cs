using ADTOSharp.Web.Configuration;

namespace ADTOSharp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ADTO Web module.
    /// </summary>
    public static class ADTOSharpWebConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ADTO Web Common module.
        /// </summary>
        public static IADTOSharpWebCommonModuleConfiguration ADTOSharpWebCommon(this IModuleConfigurations configurations)
        {
            return configurations.ADTOSharpConfiguration.Get<IADTOSharpWebCommonModuleConfiguration>();
        }
    }
}