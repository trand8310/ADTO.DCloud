using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.Quartz.Configuration
{
    public static class ADTOSharpQuartzConfigurationExtensions
    {
        /// <summary>
        ///     Used to configure ADTO Quartz module.
        /// </summary>
        public static IADTOSharpQuartzConfiguration ADTOSharpQuartz(this IModuleConfigurations configurations)
        {
            return configurations.ADTOSharpConfiguration.Get<IADTOSharpQuartzConfiguration>();
        }
    }
}
