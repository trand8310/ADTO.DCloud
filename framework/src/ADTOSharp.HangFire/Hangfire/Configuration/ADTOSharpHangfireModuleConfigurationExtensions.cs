using System;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.Hangfire.Configuration
{
    public static class ADTOSharpHangfireConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ADTO Hangfire module.
        /// </summary>
        public static IADTOSharpHangfireConfiguration ADTOSharpHangfire(this IModuleConfigurations configurations)
        {
            return configurations.ADTOSharpConfiguration.Get<IADTOSharpHangfireConfiguration>();
        }

        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration, Action<IADTOSharpHangfireConfiguration> configureAction)
        {
            backgroundJobConfiguration.ADTOSharpConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
            configureAction(backgroundJobConfiguration.ADTOSharpConfiguration.Modules.ADTOSharpHangfire());
        }
    }
}