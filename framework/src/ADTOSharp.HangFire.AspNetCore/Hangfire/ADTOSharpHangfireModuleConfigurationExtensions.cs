using ADTOSharp.BackgroundJobs;
using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.Hangfire.Configuration
{
    public static class ADTOSharpHangfireConfigurationExtensions
    {
        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            backgroundJobConfiguration.ADTOSharpConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
        }
    }
}