using System.IO;
using ADTOSharp.Dependency;
using Microsoft.Extensions.Configuration;

namespace ADTO.DCloud.Configuration
{
    /* This service is replaced in Web layer and Test project separately */
    public class DefaultAppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
    {
        public IConfigurationRoot Configuration { get; }
        
        public DefaultAppConfigurationAccessor()
        {
            Configuration = AppConfigurations.Get(Directory.GetCurrentDirectory());
        }
    }
}