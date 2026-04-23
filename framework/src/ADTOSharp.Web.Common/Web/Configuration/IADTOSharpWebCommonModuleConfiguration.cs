using ADTOSharp.Web.Api.ProxyScripting.Configuration;
using ADTOSharp.Web.MultiTenancy;
using ADTOSharp.Web.Results.Filters;
using ADTOSharp.Web.Security.AntiForgery;

namespace ADTOSharp.Web.Configuration
{
    /// <summary>
    /// Used to configure ADTO Web Common module.
    /// </summary>
    public interface IADTOSharpWebCommonModuleConfiguration
    {
        /// <summary>
        /// If this is set to true, all exception and details are sent directly to clients on an error.
        /// Default: false (ADTO hides exception details from clients except special exceptions.)
        /// </summary>
        bool SendAllExceptionsToClients { get; set; }

        /// <summary>
        /// Used to configure Api proxy scripting.
        /// </summary>
        IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        /// <summary>
        /// Used to configure Anti Forgery security settings.
        /// </summary>
        IADTOSharpAntiForgeryConfiguration AntiForgery { get; }

        /// <summary>
        /// Used to configure embedded resource system for web applications.
        /// </summary>
        IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        IWebMultiTenancyConfiguration MultiTenancy { get; }
        
        /// <summary>
        /// Used to configure wrap results
        /// </summary>
        WrapResultFilterCollection WrapResultFilters { get; }
    }
}