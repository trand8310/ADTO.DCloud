using ADTOSharp.Dependency;

namespace ADTOSharp.BlobStoring
{
    public class DefaultBlobContainerConfigurationProvider : IBlobContainerConfigurationProvider, ITransientDependency
    {
        protected ADTOSharpBlobStoringOptions Options { get; }

        public DefaultBlobContainerConfigurationProvider(ADTOSharpBlobStoringOptions options)
        {
            Options = options;
        }

        public virtual BlobContainerConfiguration Get(string name)
        {
            return Options.Containers.GetConfiguration(name);
        }
    }
}