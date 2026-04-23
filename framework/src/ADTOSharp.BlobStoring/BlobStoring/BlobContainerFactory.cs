using System;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Threading;

namespace ADTOSharp.BlobStoring
{
    public class BlobContainerFactory : IBlobContainerFactory, ITransientDependency
    {
        protected IBlobProviderSelector ProviderSelector { get; }

        protected IBlobContainerConfigurationProvider ConfigurationProvider { get; }

        protected IADTOSharpSession CurrentTenant { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected IBlobNormalizeNamingService BlobNormalizeNamingService { get; }

        public BlobContainerFactory(
            IBlobContainerConfigurationProvider configurationProvider,
            IADTOSharpSession currentTenant,
            ICancellationTokenProvider cancellationTokenProvider,
            IBlobProviderSelector providerSelector,
            IServiceProvider serviceProvider,
            IBlobNormalizeNamingService blobNormalizeNamingService)
        {
            ConfigurationProvider = configurationProvider;
            CurrentTenant = currentTenant;
            CancellationTokenProvider = cancellationTokenProvider;
            ProviderSelector = providerSelector;
            ServiceProvider = serviceProvider;
            BlobNormalizeNamingService = blobNormalizeNamingService;
        }

        public virtual IBlobContainer Create(string name)
        {
            var configuration = ConfigurationProvider.Get(name);

            return new BlobContainer(
                name,
                configuration,
                ProviderSelector.Get(name),
                CurrentTenant,
                CancellationTokenProvider,
                BlobNormalizeNamingService,
                ServiceProvider
            );
        }
    }

}
