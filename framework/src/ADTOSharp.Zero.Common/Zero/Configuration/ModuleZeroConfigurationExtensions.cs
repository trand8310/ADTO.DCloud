using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.Zero.Configuration
{
    /// <summary>
    /// Extension methods for module zero configurations.
    /// </summary>
    public static class ModuleZeroConfigurationExtensions
    {
        /// <summary>
        /// Used to configure module zero.
        /// </summary>
        /// <param name="moduleConfigurations"></param>
        /// <returns></returns>
        public static IADTOSharpZeroConfig Zero(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.ADTOSharpConfiguration.Get<IADTOSharpZeroConfig>();
        }
    }
}