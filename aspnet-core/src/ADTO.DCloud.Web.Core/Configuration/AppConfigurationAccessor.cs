using ADTO.DCloud.Configuration;
using ADTOSharp.Dependency;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;


namespace ADTO.DCloud.Web.Configuration;

public class AppConfigurationAccessor : IAppConfigurationAccessor, ISingletonDependency
{
    public IConfigurationRoot Configuration { get; }

    public AppConfigurationAccessor(IWebHostEnvironment env)
    {
        Configuration = env.GetAppConfiguration();
    }
}