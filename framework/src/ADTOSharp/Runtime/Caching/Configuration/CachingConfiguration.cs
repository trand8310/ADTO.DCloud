using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ADTOSharp.Configuration.Startup;
using Microsoft.Extensions.Caching.Memory;

namespace ADTOSharp.Runtime.Caching.Configuration
{
    internal class CachingConfiguration : ICachingConfiguration
    {
        public IADTOSharpStartupConfiguration ADTOSharpConfiguration { get; private set; }

        public IReadOnlyList<ICacheConfigurator> Configurators
        {
            get { return _configurators.ToImmutableList(); }
        }

        public MemoryCacheOptions MemoryCacheOptions { get; set; }
        
        private readonly List<ICacheConfigurator> _configurators;

        public CachingConfiguration(IADTOSharpStartupConfiguration adtoConfiguration)
        {
            ADTOSharpConfiguration = adtoConfiguration;

            _configurators = new List<ICacheConfigurator>();
        }

        public void ConfigureAll(Action<ICacheOptions> initAction)
        {
            _configurators.Add(new CacheConfigurator(initAction));
        }

        public void Configure(string cacheName, Action<ICacheOptions> initAction)
        {
            _configurators.Add(new CacheConfigurator(cacheName, initAction));
        }
    }
}