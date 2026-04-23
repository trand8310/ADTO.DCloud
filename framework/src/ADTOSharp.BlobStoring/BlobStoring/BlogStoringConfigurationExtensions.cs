using ADTOSharp.Configuration.Startup;

namespace ADTOSharp.BlobStoring
{
    public static class BlogStoringConfigurationExtensions
    {
        public static ADTOSharpBlobStoringOptions ADTOSharpBlobStoring(this IModuleConfigurations configurations)
        {
            return configurations.ADTOSharpConfiguration.Get<ADTOSharpBlobStoringOptions>();
        }
    }
}
