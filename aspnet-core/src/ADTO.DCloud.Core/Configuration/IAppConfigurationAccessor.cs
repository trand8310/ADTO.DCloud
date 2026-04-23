using Microsoft.Extensions.Configuration;

namespace ADTO.DCloud.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
