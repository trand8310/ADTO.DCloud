using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.MailKit
{
    public static class ADTOSharpMailKitConfigurationExtensions
    {
        public static IADTOSharpMailKitConfiguration ADTOSharpMailKit(this IModuleConfigurations configurations)
        {
            return configurations.ADTOSharpConfiguration.Get<IADTOSharpMailKitConfiguration>();
        }
    }
}