using ADTOSharp.Web.Api.ProxyScripting.Configuration;
using ADTOSharp.Web.MultiTenancy;
using ADTOSharp.Web.Results.Filters;
using ADTOSharp.Web.Security.AntiForgery;

namespace ADTOSharp.Web.Configuration
{
    internal class ADTOSharpWebCommonModuleConfiguration : IADTOSharpWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public IADTOSharpAntiForgeryConfiguration AntiForgery { get; }

        public IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        public IWebMultiTenancyConfiguration MultiTenancy { get; }

        public WrapResultFilterCollection WrapResultFilters { get; }

        public ADTOSharpWebCommonModuleConfiguration(
            IApiProxyScriptingConfiguration apiProxyScripting,
            IADTOSharpAntiForgeryConfiguration adtoAntiForgery,
            IWebEmbeddedResourcesConfiguration embeddedResources,
            IWebMultiTenancyConfiguration multiTenancy)
        {
            ApiProxyScripting = apiProxyScripting;
            AntiForgery = adtoAntiForgery;
            EmbeddedResources = embeddedResources;
            MultiTenancy = multiTenancy;
            WrapResultFilters = new WrapResultFilterCollection();
        }
    }
}