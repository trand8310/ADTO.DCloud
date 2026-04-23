using System;
using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.BackgroundJobs
{
    internal class BackgroundJobConfiguration : IBackgroundJobConfiguration
    {
        public static int DefaultMaxWaitingJobToProcessPerPeriod = 1000;
        
        public bool IsJobExecutionEnabled { get; set; }

        [Obsolete("Use UserTokenExpirationPeriod instead.")]
        public int? CleanUserTokenPeriod { get; set; }

        public TimeSpan? UserTokenExpirationPeriod { get; set; }

        public int MaxWaitingJobToProcessPerPeriod { get; set; } = DefaultMaxWaitingJobToProcessPerPeriod;
        
        public IADTOSharpStartupConfiguration ADTOSharpConfiguration { get; private set; }

        public BackgroundJobConfiguration(IADTOSharpStartupConfiguration adtoConfiguration)
        {
            ADTOSharpConfiguration = adtoConfiguration;

            IsJobExecutionEnabled = true;
        }
    }
}
