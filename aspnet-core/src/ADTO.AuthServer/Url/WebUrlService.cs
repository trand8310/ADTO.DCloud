using ADTO.DCloud.Configuration;
using ADTO.DCloud.Url;
using ADTO.DCloud.Web.Url;
using ADTOSharp.Dependency;

namespace ADTO.AuthServer.Url
{
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        public WebUrlService(
            IAppConfigurationAccessor configurationAccessor) :
            base(configurationAccessor)
        {
        }

        public override string WebSiteRootAddressFormatKey => "App:ClientRootAddress";

        public override string ServerRootAddressFormatKey => "App:ServerRootAddress";
    }
}