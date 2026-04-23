using ADTOSharp.Configuration.Startup;
using ADTOSharp.Localization.Dictionaries;
using ADTOSharp.Localization.Dictionaries.Xml;
using ADTOSharp.Modules;
using ADTOSharp.Web.Api.ProxyScripting.Configuration;
using ADTOSharp.Web.Api.ProxyScripting.Generators.JQuery;
using ADTOSharp.Web.Configuration;
using ADTOSharp.Web.MultiTenancy;
using ADTOSharp.Web.Security.AntiForgery;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Web.Minifier;

namespace ADTOSharp.Web
{
    /// <summary>
    /// This module is used to use ADTO in ASP.NET web applications.
    /// </summary>
    [DependsOn(typeof(ADTOSharpKernelModule))]    
    public class ADTOSharpWebCommonModule : ADTOSharpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.Register<IWebMultiTenancyConfiguration, WebMultiTenancyConfiguration>();
            IocManager.Register<IApiProxyScriptingConfiguration, ApiProxyScriptingConfiguration>();
            IocManager.Register<IADTOSharpAntiForgeryConfiguration, ADTOSharpAntiForgeryConfiguration>();
            IocManager.Register<IWebEmbeddedResourcesConfiguration, WebEmbeddedResourcesConfiguration>();
            IocManager.Register<IADTOSharpWebCommonModuleConfiguration, ADTOSharpWebCommonModuleConfiguration>();
            IocManager.Register<IJavaScriptMinifier, NUglifyJavaScriptMinifier>();

            Configuration.Modules.ADTOSharpWebCommon().ApiProxyScripting.Generators[JQueryProxyScriptGenerator.Name] = typeof(JQueryProxyScriptGenerator);

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    ADTOSharpWebConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(ADTOSharpWebCommonModule).GetAssembly(), "ADTOSharp.Web.Localization.ADTOSharpWebXmlSource"
                        )));
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpWebCommonModule).GetAssembly());            
        }
    }
}
