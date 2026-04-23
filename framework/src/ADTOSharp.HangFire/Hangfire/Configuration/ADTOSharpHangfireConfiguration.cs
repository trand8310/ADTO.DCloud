using Hangfire;
using HangfireGlobalConfiguration = Hangfire.GlobalConfiguration;

namespace ADTOSharp.Hangfire.Configuration
{
    public class ADTOSharpHangfireConfiguration : IADTOSharpHangfireConfiguration
    {
        public BackgroundJobServer Server { get; set; }

        public IGlobalConfiguration GlobalConfiguration
        {
            get { return HangfireGlobalConfiguration.Configuration; }
        }
    }
}